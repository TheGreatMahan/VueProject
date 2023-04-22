<template>
  <div class="container">
    <img class="item-image" style="margin-top: 2vh" src="../assets/cart.jpg" />
    <div class="heading">Cart Contents</div>
    <div class="status">{{ state.status }}</div>

    <div v-if="state.cart.length > 0" id="cart">
      <DataTable
        :value="state.cart"
        :scrollable="true"
        scrollHeight="38vh"
        dataKey="id"
        class="p-datatable-striped"
        id="cart-contents"
      >
        <Column
          header="Name"
          field="item.productName"
        /><!--style="margin-right: -70vw" /> -->
        <Column id="desc" header="qty" field="qty" />
        <Column id="price" header="Price" field="item.msrp" />
        <Column id="extended" header="Exnended" field="item.msrp" />
      </DataTable>

      <div v-if="state.cart.length > 0">
        <div class="cart-head">Cart Totals</div>
        <table style="border: solid; margin-top: 1vh">
          <tr>
            <td style="width: 20%; font-weight: bold">Subtotal</td>
            <td style="width: 10%; text-align: right; padding-right: 3vw">
              {{ formatCurrency(state.subTot) }}
            </td>

            <td style="width: 20%; font-weight: bold">Tax</td>
            <td style="width: 10%; text-align: right; padding-right: 3vw">
              {{ formatCurrency(state.tax) }}
            </td>

            <td style="width: 20%; font-weight: bold">Total</td>
            <td style="width: 10%; text-align: right; padding-right: 3vw">
              {{ formatCurrency(state.total) }}
            </td>
          </tr>
        </table>
        <Button
          style="margin-top: 2vh"
          label="Save Order"
          @click="saveOrder"
          class="p-button-outlined margin-button1"
        />
        &nbsp;
        <Button
          style="margin-top: 2vh"
          label="Clear Cart"
          @click="clearCart"
          class="p-button-outlined margin-button1"
        />
      </div>
    </div>
  </div>
</template>
<script>
import { reactive, onMounted } from "vue";
// import { fetcher } from "../util/apiutil";
import {poster} from "../util/apiutil";
export default {
  setup() {
    onMounted(() => {
      if (sessionStorage.getItem("cart")) {
        state.cart = JSON.parse(sessionStorage.getItem("cart"));
        state.cart.map((cartItem) => {
          state.subTot += cartItem.item.msrp * cartItem.qty;
          state.tax += cartItem.item.msrp * cartItem.qty * 0.13;
          state.total += cartItem.item.msrp * cartItem.qty * 1.13;
        });
      } else {
        state.cart = [];
      }
    });

    const clearCart = () => {
      sessionStorage.removeItem("cart");
      state.cart = [];
      state.status = "cart cleared";
    };

    let state = reactive({
      status: "",
      subTot: 0,
      tax: 0,
      total: 0,
      cart: [],
    });

    const formatCurrency = (value) => {
      return value.toLocaleString("en-US", {
        style: "currency",
        currency: "USD",
      });
    };
    const saveOrder = async () => {
      let customer = JSON.parse(sessionStorage.getItem("customer"));
      let cart = JSON.parse(sessionStorage.getItem("cart"));
      try {
        state.status = "sending order info to server";
        let orderHelper = { email: customer.email, selections: cart };
        let payload = await poster("order", orderHelper);
        if (payload.indexOf("not") > 0) {
          state.status = payload;
        } else {
          clearCart();
          state.status = payload;
        }
      } catch (err) {
        console.log(err);
        state.status = `Error add order: ${err}`;
      }
    };
    return {
      state,
      onMounted,
      clearCart,
      formatCurrency,
      saveOrder,
    };
  },
};
</script>


