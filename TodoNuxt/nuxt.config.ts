import vuetify, { transformAssetUrls } from 'vite-plugin-vuetify'
// https://nuxt.com/docs/api/configuration/nuxt-config
export default defineNuxtConfig({
  compatibilityDate: '2024-04-03',
  devtools: { enabled: true },
  modules: [
    (_options, nuxt) => {
      nuxt.hooks.hook('vite:extendConfig', (config) => {
        config.plugins.push(vuetify({ autoImport: true }))
      })
    }
  ],
  vite: {
    define: {
      'process.env': {
        BASE_API_URL: process.env.BASE_API_URL,
      }
    },
    vue: {
      template: {
        transformAssetUrls,
      },
    },
  },
  css: ['vuetify/styles'],
  build: {
    transpile: ['vuetify'],
  },
  layout: 'user-layout'
})
