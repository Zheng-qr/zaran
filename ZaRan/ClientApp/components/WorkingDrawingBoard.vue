<template>
  <div class="drawing-board">
    <!-- 工具栏 -->
    <div class="toolbar">
      <div class="tool-group">
        <button 
          v-for="tool in tools" 
          :key="tool.name"
          :class="['tool-btn', { active: currentTool === tool.name }]"
          @click="setTool(tool.name)"
        >
          {{ tool.icon }} {{ tool.label }}
        </button>
      </div>
      
      <!-- 画笔设置 -->
      <div v-if="currentTool === 'brush'" class="settings">
        <label>粗细:
          <input v-model.number="brushWidth" type="range" min="1" max="50" @input="updateBrush" />
          {{ brushWidth }}
        </label>
        <label>颜色:
          <input v-model="brushColor" type="color" @change="updateBrush" />
        </label>
        <label>类型:
          <select v-model="brushType" @change="updateBrush">
            <option value="pencil">普通画笔</option>
            <option value="pattern">图案画笔</option>
          </select>
        </label>
        <div v-if="brushType === 'pattern'" class="pattern-settings">
          <input ref="patternInput" type="file" accept="image/*" @change="loadPattern" style="display: none" />
          <button @click="$refs.patternInput?.click()" class="pattern-btn">选择图案</button>
          <span v-if="patternLoaded" class="pattern-status">✅ 图案已加载</span>
        </div>
      </div>
      
      <!-- 形状设置 -->
      <div v-if="currentTool === 'shape'" class="settings">
        <select v-model="shapeType">
          <option value="rect">长方形</option>
          <option value="circle">圆形</option>
        </select>
        <label>颜色: 
          <input v-model="shapeFill" type="color" />
        </label>
      </div>
      
      <!-- 橡皮擦设置 -->
      <div v-if="currentTool === 'eraser'" class="settings">
        <label>大小:
          <input v-model.number="eraserWidth" type="range" min="5" max="50" @input="updateEraser" />
          {{ eraserWidth }}
        </label>
      </div>

      <!-- 文字设置 -->
      <div v-if="currentTool === 'text'" class="settings">
        <label>颜色:
          <input v-model="textColor" type="color" />
        </label>
        <label>大小:
          <input v-model.number="textSize" type="range" min="12" max="72" />
          {{ textSize }}
        </label>
      </div>

      <!-- 操作按钮 -->
      <div class="tool-group">
        <label>背景:
          <input v-model="backgroundColor" type="color" @change="setBackground" />
        </label>
        <button @click="zoomIn">🔍+</button>
        <span>{{ Math.round(zoom * 100) }}%</span>
        <button @click="zoomOut">🔍-</button>
        <button @click="resetZoom">100%</button>
      </div>

      <div class="tool-group">
        <input ref="imageInput" type="file" accept="image/*" @change="addImage" style="display: none" />
        <button @click="$refs.imageInput?.click()">📷 图片</button>
        <button @click="clearCanvas">🗑️ 清空</button>
        <button @click="undo" :disabled="!canUndo">↶ 撤销</button>
        <button @click="redo" :disabled="!canRedo">↷ 重做</button>
      </div>

      <div class="tool-group">
        <button @click="exportImage">💾 导出图片</button>
        <button @click="exportData">📤 导出数据</button>
        <input ref="dataInput" type="file" accept=".json" @change="importData" style="display: none" />
        <button @click="$refs.dataInput?.click()">📥 导入数据</button>
      </div>
    </div>
    
    <!-- 画布容器 -->
    <div class="canvas-container">
      <canvas ref="canvasRef"></canvas>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, onMounted, nextTick } from 'vue'

// 引用
const canvasRef = ref<HTMLCanvasElement>()
const imageInput = ref<HTMLInputElement>()
const dataInput = ref<HTMLInputElement>()
const patternInput = ref<HTMLInputElement>()

// 画布实例
let canvas: any = null

// 工具状态
const currentTool = ref('brush')
const tools = [
  { name: 'select', label: '选择', icon: '🔍' },
  { name: 'brush', label: '画笔', icon: '🖌️' },
  { name: 'eraser', label: '橡皮擦', icon: '🧽' },
  { name: 'shape', label: '形状', icon: '🔷' },
  { name: 'text', label: '文字', icon: '📝' }
]

// 设置参数
const brushWidth = ref(5)
const brushColor = ref('#000000')
const brushType = ref('pencil')
const patternLoaded = ref(false)
let patternBrush: any = null
const eraserWidth = ref(10)
const shapeType = ref('rect')
const shapeFill = ref('#ff0000')
const textColor = ref('#000000')
const textSize = ref(20)
const backgroundColor = ref('#ffffff')
const zoom = ref(1)

// 历史记录
const history = ref<string[]>([])
const historyIndex = ref(-1)
const canUndo = ref(false)
const canRedo = ref(false)

// 初始化画布
const initCanvas = async () => {
  if (!canvasRef.value) return
  
  try {
    // 动态导入 Fabric.js
    const fabric = await import('fabric')
    const { Canvas, PencilBrush } = fabric
    
    canvas = new Canvas(canvasRef.value, {
      width: 800,
      height: 600
    })

    // 设置背景色
    canvas.backgroundColor = backgroundColor.value
    
    // 设置默认为画笔模式
    canvas.isDrawingMode = true
    canvas.freeDrawingBrush = new PencilBrush(canvas)
    canvas.freeDrawingBrush.width = brushWidth.value
    canvas.freeDrawingBrush.color = brushColor.value

    // 设置事件监听
    canvas.on('object:modified', saveState)
    canvas.on('path:created', saveState)
    canvas.on('object:added', saveState)
    canvas.on('object:removed', saveState)

    // 保存初始状态
    saveState()

    console.log('Canvas initialized successfully', {
      isDrawingMode: canvas.isDrawingMode,
      brushWidth: canvas.freeDrawingBrush.width,
      brushColor: canvas.freeDrawingBrush.color
    })
    
  } catch (error) {
    console.error('Failed to initialize canvas:', error)
  }
}

// 设置工具
const setTool = async (tool: string) => {
  currentTool.value = tool
  if (!canvas) return

  // 清理图案绘制事件
  cleanupPatternEvents()

  const fabric = await import('fabric')
  const { PencilBrush } = fabric

  // 重置画布模式
  canvas.isDrawingMode = false
  canvas.selection = true

  switch (tool) {
    case 'select':
      canvas.selection = true
      console.log('Selection mode enabled')
      break
    case 'brush':
      canvas.isDrawingMode = true
      if (!canvas.freeDrawingBrush || canvas.freeDrawingBrush.type !== 'PencilBrush') {
        canvas.freeDrawingBrush = new PencilBrush(canvas)
      }
      canvas.freeDrawingBrush.width = brushWidth.value
      canvas.freeDrawingBrush.color = brushColor.value
      console.log('Drawing mode enabled', {
        width: canvas.freeDrawingBrush.width,
        color: canvas.freeDrawingBrush.color
      })
      break
    case 'eraser':
      canvas.isDrawingMode = true
      // 使用白色画笔作为橡皮擦
      canvas.freeDrawingBrush = new PencilBrush(canvas)
      canvas.freeDrawingBrush.width = eraserWidth.value
      canvas.freeDrawingBrush.color = backgroundColor.value
      console.log('Eraser mode enabled (using white brush)')
      break
    case 'shape':
      setupShapeMode(fabric)
      break
    case 'text':
      setupTextMode(fabric)
      break
  }
}

// 更新画笔
const updateBrush = async () => {
  if (!canvas) return

  const fabric = await import('fabric')
  const { PencilBrush } = fabric

  // 始终使用普通画笔，但在图案模式下禁用自由绘制
  if (!canvas.freeDrawingBrush || canvas.freeDrawingBrush.constructor.name !== 'PencilBrush') {
    canvas.freeDrawingBrush = new PencilBrush(canvas)
  }

  if (brushType.value === 'pattern' && patternImage) {
    // 图案模式：禁用自由绘制，使用自定义绘制逻辑
    canvas.isDrawingMode = false
    canvas.selection = false // 禁用选择模式

    // 总是重新设置图案绘制事件，确保事件正确绑定
    cleanupPatternEvents()
    setupPatternDrawing()

    console.log('Pattern mode activated', {
      isDrawingMode: canvas.isDrawingMode,
      selection: canvas.selection,
      hasPatternImage: !!patternImage
    })
  } else {
    // 普通画笔模式：清理图案事件，启用自由绘制
    cleanupPatternEvents()
    canvas.isDrawingMode = true
    canvas.freeDrawingBrush.width = brushWidth.value
    canvas.freeDrawingBrush.color = brushColor.value
  }

  console.log('Brush updated:', {
    type: brushType.value,
    width: brushWidth.value,
    color: brushColor.value,
    hasPattern: !!patternImage,
    isDrawingMode: canvas.isDrawingMode,
    hasPatternEvents: !!patternEventHandlers.onMouseDown
  })
}

// 设置图案绘制模式
const setupPatternDrawing = () => {
  if (!canvas || !patternImage) return

  let isDrawing = false
  let lastPoint: any = null

  const onMouseDown = (e: any) => {
    console.log('Pattern onMouseDown triggered', {
      brushType: brushType.value,
      hasPatternImage: !!patternImage,
      isDrawingMode: canvas.isDrawingMode
    })

    if (brushType.value !== 'pattern') return

    isDrawing = true
    const pointer = canvas.getPointer(e.e)
    lastPoint = pointer

    // 开始绘制图案
    drawPatternAt(pointer.x, pointer.y)
    console.log('Pattern drawing started at', pointer)
  }

  const onMouseMove = (e: any) => {
    if (!isDrawing || brushType.value !== 'pattern') return

    const pointer = canvas.getPointer(e.e)

    // 在移动路径上绘制图案
    if (lastPoint) {
      const distance = Math.sqrt(
        Math.pow(pointer.x - lastPoint.x, 2) + Math.pow(pointer.y - lastPoint.y, 2)
      )

      // 每隔一定距离绘制一个图案
      if (distance > brushWidth.value / 4) {
        drawPatternAt(pointer.x, pointer.y)
        lastPoint = pointer
      }
    }
  }

  const onMouseUp = () => {
    isDrawing = false
    lastPoint = null

    // 完成图案路径绘制
    finishPatternPath()

    // 保存状态
    saveState()
  }

  // 保存事件处理器引用
  patternEventHandlers = {
    onMouseDown,
    onMouseMove,
    onMouseUp
  }

  // 绑定事件
  canvas.on('mouse:down', onMouseDown)
  canvas.on('mouse:move', onMouseMove)
  canvas.on('mouse:up', onMouseUp)
}

// 在指定位置记录图案点
const drawPatternAt = (x: number, y: number) => {
  if (!canvas || !patternImage) return

  // 记录图案点到当前路径
  currentPatternPath.push({
    x: x,
    y: y,
    size: brushWidth.value
  })

  // 在临时画布上绘制预览
  drawPatternPreview(x, y)
}

// 在临时画布上绘制图案预览
const drawPatternPreview = (x: number, y: number) => {
  if (!canvas || !patternImage) return

  const ctx = canvas.contextTop
  if (ctx) {
    ctx.save()

    // 绘制图案
    const size = brushWidth.value
    ctx.drawImage(
      patternImage,
      x - size / 2,
      y - size / 2,
      size,
      size
    )

    ctx.restore()
  }
}

// 完成图案路径绘制
const finishPatternPath = async () => {
  if (!canvas || !patternImage || currentPatternPath.length === 0) return

  try {
    const fabric = await import('fabric')
    const { Group, FabricImage } = fabric

    // 创建图案组
    const patternObjects: any[] = []

    for (const point of currentPatternPath) {
      // 为每个点创建新的图案对象
      const patternObj = new FabricImage(patternImage, {
        left: point.x - point.size / 2,
        top: point.y - point.size / 2,
        scaleX: point.size / patternImage.width,
        scaleY: point.size / patternImage.height,
        selectable: false,
        evented: false
      })

      patternObjects.push(patternObj)
    }

    // 创建组对象
    const patternGroup = new Group(patternObjects, {
      selectable: true,
      evented: true
    })

    // 添加到画布
    canvas.add(patternGroup)

    // 清空临时画布
    canvas.clearContext(canvas.contextTop)

    // 重新渲染
    canvas.renderAll()

    // 清空当前路径
    currentPatternPath = []

    console.log('Pattern path finished with', patternObjects.length, 'objects')

  } catch (error) {
    console.error('Error finishing pattern path:', error)

    // 降级方案：直接将临时画布内容转换为图片
    try {
      const dataURL = canvas.contextTop.canvas.toDataURL()
      const fabric = await import('fabric')
      const { FabricImage } = fabric

      const img = new Image()
      img.onload = () => {
        const fabricImg = new FabricImage(img, {
          left: 0,
          top: 0,
          selectable: true
        })
        canvas.add(fabricImg)
        canvas.clearContext(canvas.contextTop)
        canvas.renderAll()
      }
      img.src = dataURL

      // 清空当前路径
      currentPatternPath = []

    } catch (fallbackError) {
      console.error('Fallback also failed:', fallbackError)
    }
  }
}

// 图案画笔数据
let patternImage: HTMLImageElement | null = null
// let patternCanvas: HTMLCanvasElement | null = null // 暂未使用

// 图案绘制事件处理器引用
let patternEventHandlers: {
  onMouseDown?: any
  onMouseMove?: any
  onMouseUp?: any
} = {}

// 当前绘制的图案路径
let currentPatternPath: any[] = []

// 创建图案（暂未使用）
// const createPattern = (img: HTMLImageElement, size: number) => {
//   patternCanvas = document.createElement('canvas')
//   const ctx = patternCanvas.getContext('2d')

//   if (!ctx) return null

//   // 设置图案大小
//   patternCanvas.width = size
//   patternCanvas.height = size

//   // 绘制图案
//   ctx.drawImage(img, 0, 0, size, size)

//   // 创建图案
//   return ctx.createPattern(patternCanvas, 'repeat')
// }

// 清理图案绘制事件
const cleanupPatternEvents = () => {
  if (!canvas) return

  if (patternEventHandlers.onMouseDown) {
    canvas.off('mouse:down', patternEventHandlers.onMouseDown)
  }
  if (patternEventHandlers.onMouseMove) {
    canvas.off('mouse:move', patternEventHandlers.onMouseMove)
  }
  if (patternEventHandlers.onMouseUp) {
    canvas.off('mouse:up', patternEventHandlers.onMouseUp)
  }

  patternEventHandlers = {}
}

// 加载图案
const loadPattern = (event: Event) => {
  const file = (event.target as HTMLInputElement).files?.[0]
  if (!file || !canvas) return

  const reader = new FileReader()
  reader.onload = async (e) => {
    try {
      const imgUrl = e.target?.result as string

      const imgElement = new Image()
      imgElement.onload = async () => {
        // 保存图案图片
        patternImage = imgElement

        // 创建复用的 Fabric 图片对象
        const fabric = await import('fabric')
        const { FabricImage } = fabric

        // 创建 Fabric 图片对象用于图案画笔
        const patternFabricImage = new FabricImage(imgElement, {
          selectable: false,
          evented: false
        })

        patternLoaded.value = true

        // 如果当前是图案模式，立即应用
        if (brushType.value === 'pattern') {
          updateBrush()
        }

        console.log('Pattern loaded successfully')
      }

      imgElement.onerror = () => {
        console.error('Failed to load pattern image')
        alert('图案加载失败，请尝试其他图片')
      }

      imgElement.src = imgUrl
    } catch (error) {
      console.error('Error loading pattern:', error)
      alert('加载图案失败')
    }
  }
  reader.readAsDataURL(file)

  // 清空文件输入
  ;(event.target as HTMLInputElement).value = ''
}

// 更新橡皮擦
const updateEraser = () => {
  if (!canvas || !canvas.freeDrawingBrush) return
  canvas.freeDrawingBrush.width = eraserWidth.value
}

// 缩放功能
const zoomIn = () => {
  if (!canvas) return
  zoom.value = Math.min(zoom.value * 1.1, 3)
  canvas.setZoom(zoom.value)
}

const zoomOut = () => {
  if (!canvas) return
  zoom.value = Math.max(zoom.value / 1.1, 0.1)
  canvas.setZoom(zoom.value)
}

const resetZoom = () => {
  if (!canvas) return
  zoom.value = 1
  canvas.setZoom(1)
  canvas.viewportTransform = [1, 0, 0, 1, 0, 0]
}

// 设置背景
const setBackground = () => {
  if (!canvas) return
  canvas.backgroundColor = backgroundColor.value
  canvas.renderAll()
  saveState()
}

// 添加图片
const addImage = (event: Event) => {
  const file = (event.target as HTMLInputElement).files?.[0]
  if (!file || !canvas) return

  const reader = new FileReader()
  reader.onload = async (e) => {
    try {
      const fabric = await import('fabric')
      const { FabricImage } = fabric
      const imgUrl = e.target?.result as string

      // 创建图片对象
      const imgElement = new Image()
      imgElement.onload = () => {
        const fabricImg = new FabricImage(imgElement, {
          left: 100,
          top: 100,
          scaleX: 0.5,
          scaleY: 0.5
        })

        // 保存原始 base64 数据用于导出
        fabricImg.set('originalSrc', imgUrl)

        canvas.add(fabricImg)
        canvas.setActiveObject(fabricImg)
        canvas.renderAll()
        saveState()

        console.log('Image added successfully')
      }

      imgElement.onerror = () => {
        console.error('Failed to load image')
        alert('图片加载失败，请尝试其他图片')
      }

      imgElement.src = imgUrl
    } catch (error) {
      console.error('Error adding image:', error)
      alert('添加图片失败')
    }
  }
  reader.readAsDataURL(file)

  // 清空文件输入，允许重复选择同一文件
  ;(event.target as HTMLInputElement).value = ''
}

// 历史记录管理
const saveState = () => {
  if (!canvas) return

  const state = JSON.stringify(canvas.toJSON())
  history.value = history.value.slice(0, historyIndex.value + 1)
  history.value.push(state)
  historyIndex.value = history.value.length - 1

  if (history.value.length > 50) {
    history.value.shift()
    historyIndex.value--
  }

  updateHistoryButtons()
}

const undo = () => {
  if (!canvas || historyIndex.value <= 0) return

  historyIndex.value--
  const state = history.value[historyIndex.value]
  canvas.loadFromJSON(state, () => {
    canvas.renderAll()
    updateHistoryButtons()
  })
}

const redo = () => {
  if (!canvas || historyIndex.value >= history.value.length - 1) return

  historyIndex.value++
  const state = history.value[historyIndex.value]
  canvas.loadFromJSON(state, () => {
    canvas.renderAll()
    updateHistoryButtons()
  })
}

const updateHistoryButtons = () => {
  canUndo.value = historyIndex.value > 0
  canRedo.value = historyIndex.value < history.value.length - 1
}

// 导出数据（下载文件）
const exportData = () => {
  if (!canvas) return

  const data = getCanvasData()
  const jsonString = JSON.stringify(data, null, 2)
  const blob = new Blob([jsonString], { type: 'application/json' })
  const url = URL.createObjectURL(blob)

  const link = document.createElement('a')
  link.download = 'drawing.json'
  link.href = url
  link.click()

  URL.revokeObjectURL(url)

  console.log('Data exported with image base64 included')
}

// 获取画布数据（不下载，供外部调用）
const getCanvasData = () => {
  if (!canvas) return null

  // 导出包含图片 base64 数据的完整数据
  const data = canvas.toJSON(['originalSrc'])

  // 确保图片的 base64 数据被包含
  const objects = canvas.getObjects()
  objects.forEach((obj: any, index: number) => {
    if (obj.type === 'image' && obj.originalSrc) {
      data.objects[index].originalSrc = obj.originalSrc
    }
  })

  return data
}

// 添加图案到画布（供外部调用）
const addPatternToCanvas = async (pattern: any) => {
  if (!canvas) return

  try {
    const fabric = await import('fabric')

    if (pattern.svgPath) {
      // SVG 图案处理 - 使用 Path 对象而不是 loadSVGFromString
      const pathObj = new fabric.Path(pattern.svgPath, {
        left: canvas.width / 2,
        top: canvas.height / 2,
        originX: 'center',
        originY: 'center',
        fill: brushColor.value,
        scaleX: 1.5,
        scaleY: 1.5,
        name: pattern.name
      })

      canvas.add(pathObj)
      canvas.setActiveObject(pathObj)
      canvas.renderAll()
      saveState()
    } else if (pattern.image) {
      // 图片图案处理
      const imgElement = new Image()
      imgElement.crossOrigin = 'anonymous'
      imgElement.onload = () => {
        const fabricImg = new fabric.FabricImage(imgElement, {
          left: canvas.width / 2,
          top: canvas.height / 2,
          originX: 'center',
          originY: 'center',
          scaleX: 0.6,
          scaleY: 0.6
        })

        // 保存原始图片数据
        fabricImg.set('originalSrc', pattern.image)

        canvas.add(fabricImg)
        canvas.setActiveObject(fabricImg)
        canvas.renderAll()
        saveState()
      }
      imgElement.onerror = () => {
        console.error('Failed to load pattern image:', pattern.image)
      }
      imgElement.src = pattern.image
    } else {
      // 简单图标处理 - 创建一个带图标的文本对象
      const iconText = new fabric.FabricText(pattern.icon || '●', {
        left: canvas.width / 2,
        top: canvas.height / 2,
        originX: 'center',
        originY: 'center',
        fontSize: 40,
        fill: brushColor.value,
        fontFamily: 'FontAwesome',
        name: pattern.name
      })

      canvas.add(iconText)
      canvas.setActiveObject(iconText)
      canvas.renderAll()
      saveState()
    }
  } catch (error) {
    console.error('Error adding pattern to canvas:', error)
    throw error // 重新抛出错误以便上层处理
  }
}

// 导入数据
const importData = (event: Event) => {
  const file = (event.target as HTMLInputElement).files?.[0]
  if (!file || !canvas) return

  const reader = new FileReader()
  reader.onload = async (e) => {
    try {
      const data = JSON.parse(e.target?.result as string)

      // 处理图片对象，恢复 base64 数据
      if (data.objects) {
        for (let i = 0; i < data.objects.length; i++) {
          const obj = data.objects[i]
          if (obj.type === 'image' && obj.originalSrc) {
            // 预加载图片以确保正确显示
            const imgElement = new Image()
            imgElement.src = obj.originalSrc
            await new Promise((resolve) => {
              imgElement.onload = resolve
              imgElement.onerror = resolve
            })
          }
        }
      }

      canvas.loadFromJSON(data, () => {
        // 恢复图片的原始 base64 数据
        const objects = canvas.getObjects()
        objects.forEach((obj: any, index: number) => {
          if (obj.type === 'image' && data.objects[index]?.originalSrc) {
            obj.set('originalSrc', data.objects[index].originalSrc)
          }
        })

        canvas.renderAll()
        saveState()
        console.log('Data imported with images restored')
      })
    } catch (error) {
      console.error('导入失败:', error)
      alert('导入失败，请检查文件格式')
    }
  }
  reader.readAsText(file)
}

// 设置形状模式
const setupShapeMode = (fabric: any) => {
  if (!canvas) return
  
  const { Rect, Circle } = fabric
  
  let isDrawing = false
  let startPointer: any
  let shape: any
  
  const onMouseDown = (e: any) => {
    if (currentTool.value !== 'shape') return
    
    isDrawing = true
    startPointer = canvas.getPointer(e.e)
    
    const options = {
      left: startPointer.x,
      top: startPointer.y,
      fill: shapeFill.value,
      stroke: '#000000',
      strokeWidth: 1
    }
    
    if (shapeType.value === 'rect') {
      shape = new Rect({ ...options, width: 0, height: 0 })
    } else {
      shape = new Circle({ ...options, radius: 0 })
    }
    
    canvas.add(shape)
  }
  
  const onMouseMove = (e: any) => {
    if (!isDrawing) return
    
    const pointer = canvas.getPointer(e.e)
    const width = Math.abs(pointer.x - startPointer.x)
    const height = Math.abs(pointer.y - startPointer.y)
    
    if (shapeType.value === 'rect') {
      shape.set({ width, height })
    } else {
      shape.set({ radius: Math.max(width, height) / 2 })
    }
    
    canvas.renderAll()
  }
  
  const onMouseUp = () => {
    isDrawing = false
    canvas.off('mouse:down', onMouseDown)
    canvas.off('mouse:move', onMouseMove)
    canvas.off('mouse:up', onMouseUp)
  }
  
  canvas.on('mouse:down', onMouseDown)
  canvas.on('mouse:move', onMouseMove)
  canvas.on('mouse:up', onMouseUp)
}

// 设置文字模式
const setupTextMode = (fabric: any) => {
  if (!canvas) return
  
  const { IText } = fabric
  
  const onMouseDown = (e: any) => {
    if (currentTool.value !== 'text') return
    
    const pointer = canvas.getPointer(e.e)
    const text = new IText('输入文字', {
      left: pointer.x,
      top: pointer.y,
      fill: '#000000',
      fontSize: 20
    })
    
    canvas.add(text)
    canvas.setActiveObject(text)
    text.enterEditing()
    
    canvas.off('mouse:down', onMouseDown)
  }
  
  canvas.on('mouse:down', onMouseDown)
}

// 清空画布
const clearCanvas = () => {
  if (!canvas) return
  canvas.clear()
  canvas.backgroundColor = backgroundColor.value
  canvas.renderAll()
  saveState()
}

// 导出图片
const exportImage = () => {
  if (!canvas) return
  
  const dataURL = canvas.toDataURL({
    format: 'png',
    quality: 1
  })
  
  const link = document.createElement('a')
  link.download = 'drawing.png'
  link.href = dataURL
  link.click()
}

// 生命周期
onMounted(() => {
  nextTick(() => {
    initCanvas()
  })
})

// 暴露方法给父组件
defineExpose({
  exportData: getCanvasData,
  addPattern: addPatternToCanvas,
  clearCanvas,
  exportImage
})
</script>

<style scoped>
.drawing-board {
  display: flex;
  flex-direction: column;
  height: 100vh;
  background-color: #f5f5f5;
}

.toolbar {
  display: flex;
  flex-wrap: wrap;
  gap: 10px;
  padding: 10px;
  background-color: #fff;
  border-bottom: 1px solid #ddd;
  box-shadow: 0 2px 4px rgba(0,0,0,0.1);
}

.tool-group {
  display: flex;
  align-items: center;
  gap: 5px;
  padding: 5px;
  border: 1px solid #ddd;
  border-radius: 4px;
  background-color: #fafafa;
}

.tool-btn {
  padding: 8px 12px;
  border: 1px solid #ccc;
  border-radius: 4px;
  background-color: #fff;
  cursor: pointer;
  transition: all 0.2s;
  font-size: 12px;
}

.tool-btn:hover {
  background-color: #e6f3ff;
  border-color: #0066cc;
}

.tool-btn.active {
  background-color: #0066cc;
  color: white;
  border-color: #0066cc;
}

.settings {
  display: flex;
  align-items: center;
  gap: 10px;
  padding: 5px;
  background-color: #f0f0f0;
  border-radius: 4px;
}

.settings label {
  display: flex;
  align-items: center;
  gap: 5px;
  font-size: 12px;
  white-space: nowrap;
}

.settings input[type="range"] {
  width: 80px;
}

.settings input[type="color"] {
  width: 30px;
  height: 25px;
  border: none;
  border-radius: 3px;
  cursor: pointer;
}

.settings select {
  padding: 2px 5px;
  border: 1px solid #ccc;
  border-radius: 3px;
  font-size: 12px;
}

.canvas-container {
  flex: 1;
  position: relative;
  overflow: auto;
  background-color: #e0e0e0;
  display: flex;
  justify-content: center;
  align-items: center;
  padding: 20px;
}

.canvas-container canvas {
  border: 2px solid #333;
  background-color: white;
  box-shadow: 0 4px 8px rgba(0,0,0,0.2);
}

.pattern-settings {
  display: flex;
  align-items: center;
  gap: 8px;
  margin-top: 5px;
}

.pattern-btn {
  padding: 4px 8px;
  border: 1px solid #ccc;
  border-radius: 3px;
  background: white;
  cursor: pointer;
  font-size: 11px;
}

.pattern-btn:hover {
  background: #f0f0f0;
}

.pattern-status {
  font-size: 11px;
  color: #059669;
  font-weight: 500;
}
</style>
