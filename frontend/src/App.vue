<template>
  <div>
    <Toolbar>
      <template #left>
        <span style="font-weight: bold; font-size: larger"
          >Info3139 Casestudy</span
        >
      </template>
      <template #right>
        <Menu id="overlay_menu" ref="menu" :model="products" :popup="true" />
        <Button icon="pi pi-bars" @click="toggle" />
      </template>
    </Toolbar>

    <router-view />
    <div class="footer">
      best viewed on mobile device — Mahan Mehdipour &copy;{{
        new Date().getFullYear()
      }}
    </div>
  </div>
</template>
<script>
import { ref } from "vue";
export default {
  setup() {
    const products = ref([
      { label: "Home", to: "/" },
      { label: "Login", to: "/login" },
      { label: "Register", to: "/register" },
      { label: "Brands", to: "/brands" },
      { label: "Cart", to: "/cart" },
      { label: "Order List", to: "/orderlist" },
      { label: "Branches", to: "/branch" },
      { label: "Logout", to: "/logout" },


    ]);
    const menu = ref();
    const toggle = (event) => {
      menu.value.toggle(event);
    };
    return {
      products,
      menu,
      toggle,
    };
  },
};
</script>
<style>
#app {
  font-family: Avenir, Helvetica, Arial, sans-serif;
  -webkit-font-smoothing: antialiased;
  -moz-osx-font-smoothing: grayscale;
  text-align: center;
  color: #2c3e50;
  margin-top: 60px;
}
</style>