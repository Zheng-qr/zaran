// https://nuxt.com/docs/api/configuration/nuxt-config
export default defineNuxtConfig({
  devtools: { enabled: true },
  modules: ['@nuxtjs/tailwindcss', '@pinia/nuxt', '@vueuse/nuxt'],

  // Auto-imports configuration
  imports: {
    dirs: [
      'composables/**',
      'stores/**'
    ]
  },

  // TypeScript configuration
  typescript: {
    typeCheck: false
  },

  // Development server configuration
  devServer: {
    port: 3000,
    host: 'localhost'
  },

  // Server configuration
  nitro: {
    devProxy: {
      '/api': {
        target: 'http://localhost:5127',
        changeOrigin: true,
        secure: false,
      }
    }
  },

  // CSS configuration
  css: ['~/assets/css/main.css'],

  // Vite configuration
  vite: {
    define: {
      global: 'globalThis',
    },
    optimizeDeps: {
      include: ['axios', 'form-data']
    },
    ssr: {
      noExternal: ['form-data']
    }
  },

  // App configuration
  app: {
    head: {
      title: 'ZaRan',
      meta: [
        { charset: 'utf-8' },
        { name: 'viewport', content: 'width=device-width, initial-scale=1' }
      ],
      link: [
        { rel: 'stylesheet', href: 'https://cdnjs.cloudflare.com/ajax/libs/font-awesome/6.0.0/css/all.min.css' }
      ]
    }
  },

  compatibilityDate: '2025-07-28'
})