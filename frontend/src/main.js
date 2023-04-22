import { createApp } from 'vue'
import App from './App.vue'
import router from "./router";
import PrimeVue from 'primevue/config';
import Button from 'primevue/button';
import Dropdown from 'primevue/dropdown';
import DataTable from 'primevue/DataTable';
import Column from 'primevue/Column';
import Dialog from 'primevue/Dialog';
import InputNumber from 'primevue/InputNumber';
import Toolbar from 'primevue/Toolbar';
import Menu from 'primevue/Menu';
import InputText from 'primevue/InputText';



import 'primevue/resources/primevue.min.css'; //core css
import 'primevue/resources/themes/md-light-deeppurple/theme.css'; //theme
import 'primeicons/primeicons.css'; //icons
import './assets/site.css';
const app = createApp(App)
app.use(PrimeVue);
app.use(router);

app.component('Button', Button);
app.component('Dropdown', Dropdown);
app.component('DataTable', DataTable);
app.component('Column', Column);
app.component('Dialog',Dialog);
app.component('InputNumber',InputNumber);
app.component('Toolbar', Toolbar);
app.component('Menu',Menu);
app.component('InputText', InputText);

app.mount('#app');
