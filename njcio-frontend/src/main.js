import { createApp } from 'vue'
import { createPinia } from 'pinia'
import App from './App.vue'

import { createVuetify } from 'vuetify'
import * as components from 'vuetify/components'
import * as directives from 'vuetify/directives'
import '@mdi/font/css/materialdesignicons.css'
import 'vuetify/dist/vuetify.css'
import '@fontsource/jetbrains-mono'
import './assets/main.css'

const vuetify = createVuetify({
	components,
	directives,
	defaults: {
		VBtn: {
			style: 'text-transform: none;'
		}
	},
	theme: {
    defaultTheme: 'light',
    themes: {
      light: {
        colors: {
          background: '#f0e4c8'
        }
      }
    }
  }
})

createApp(App)
	.use(vuetify)
	.use(createPinia())
	.mount('#app')
