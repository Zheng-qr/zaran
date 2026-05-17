// 测试消息API的脚本
const baseUrl = 'http://localhost:5127/api';

async function testLogin() {
  try {
    const response = await fetch(`${baseUrl}/user/login`, {
      method: 'POST',
      headers: {
        'Content-Type': 'application/json',
      },
      body: JSON.stringify({
        username: 'admin',
        password: 'admin123'
      })
    });

    if (response.ok) {
      const data = await response.json();
      console.log('登录成功:', data);
      return data.token;
    } else {
      console.error('登录失败:', response.status, await response.text());
      return null;
    }
  } catch (error) {
    console.error('登录请求失败:', error);
    return null;
  }
}

async function testMessages(token) {
  try {
    const response = await fetch(`${baseUrl}/messages?offset=0&limit=20&desc=true`, {
      headers: {
        'Authorization': `Bearer ${token}`,
        'Content-Type': 'application/json',
      }
    });

    if (response.ok) {
      const data = await response.json();
      console.log('获取消息成功:', data);
      return data;
    } else {
      console.error('获取消息失败:', response.status, await response.text());
      return null;
    }
  } catch (error) {
    console.error('获取消息请求失败:', error);
    return null;
  }
}

async function runTest() {
  console.log('开始测试...');
  
  // 测试登录
  const token = await testLogin();
  if (!token) {
    console.log('登录失败，无法继续测试');
    return;
  }

  // 测试获取消息
  const messages = await testMessages(token);
  if (messages) {
    console.log(`成功获取 ${messages.items?.length || 0} 条消息`);
  }
}

// 在Node.js环境中运行
if (typeof window === 'undefined') {
  // Node.js环境
  const fetch = require('node-fetch');
  runTest();
} else {
  // 浏览器环境
  window.testMessageAPI = runTest;
}
