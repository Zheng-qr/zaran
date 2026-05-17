/** @type {import('tailwindcss').Config} */
export default {
  content: [
    "./components/**/*.{js,vue,ts}",
    "./layouts/**/*.vue",
    "./pages/**/*.vue",
    "./plugins/**/*.{js,ts}",
    "./app.vue",
    "./error.vue"
  ],
  theme: {
    extend: {
      colors: {
        primary: '#076cd1',
        secondary: '#00A896',
        accent1: '#F7B733',
        accent2: '#E85A4F',
        accent3: '#8E44AD',
        light: '#f3f5f8',
        'medium-gray': '#999ea6',
        'dark-gray': '#6c757d',
        dark: '#2c3e50',
      }
    },
  },
  plugins: [
    require('@tailwindcss/typography'),
  ],
}
