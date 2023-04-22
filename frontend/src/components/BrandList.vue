<template>
  <div>
    <div class="heading">Our Brands</div>
    <div class="status">{{ state.status }}</div>
    <Dropdown
      v-if="state.brands.length > 0"
      v-model="state.selectedBrand"
      style="text-align: left"
      :options="state.brands"
      optionLabel="name"
      optionValue="id"
      placeholder="Select Brand"
      @change="loadProducts"
    />
    <DataTable
      v-if="state.products.length > 0"
      :value="state.products"
      :scrollable="true"
      scrollHeight="55vh"
      selectionMode="single"
      class="p-datatable-striped"
      @row-select="onRowSelect"
    >
      <Column style="margin-right: -35vw">
        <template #body="slotProps">
          <img
            style="width: 30%"
            :src="`/img/${slotProps.data.graphicName}`"
            :alt="slotProps.data.graphicName"
            class="product-image"
          />
        </template>
      </Column>
      <Column field="productName" header="Products"></Column>
    </DataTable>
    <Dialog v-model:visible="state.itemSelected" class="dialog-border">
      <div style="text-align: center">
        <img
          style="width: 50%"
          :src="`/img/${state.selectedItem.graphicName}`"
          :alt="state.selectedItem.graphicName"
          class="product-image"
        />
      </div>
      <div
        style="margin-top:2vh margin-bottom: 5vh; font-size: larger; fontweight: bold"
      >
        {{ state.selectedItem.productName }} -
        {{ formatCurrency(state.selectedItem.msrp) }}
      </div>
      <div style="margin-top: 2vh">
        {{ state.selectedItem.description }}
      </div>
      <div style="margin-top: 2vh; text-align: center">
        <div style="margin-left: -10vw; margin-right: 2vw margin-bottom: 10vh">
          Enter Qty:
        </div>
        <div>
          <InputNumber
            id="qty"
            :min="0"
            v-model="state.qty"
            :step="1"
            incrementButtonClass="plus"
            showButtons
            buttonLayout="horizontal"
            decrementButtonIcon="pi pi-minus"
            incrementButtonIcon="pi pi-plus"
          />
        </div>
      </div>
      <div style="text-align: center; margin-top: 2vh">
        <Button
          label="Add To Cart"
          @click="addToCart"
          class="p-button-outlined margin-button1"
        />
        &nbsp;
        <Button
          label="View Cart"
          class="p-button-outlined margin-button1"
          v-if="state.cart.length > 0"
          @click="viewCart"
        />
      </div>
      <div
        style="text-align: center"
        v-if="state.dialogStatus !== ''"
        class="dialog-status"
      >
        {{ state.dialogStatus }}
      </div>
    </Dialog>
  </div>
</template>
<script>
import { reactive, onMounted } from "vue";
import { fetcher } from "../util/apiutil";
import { useRouter } from "vue-router";
export default {
  setup() {
    const router = useRouter();
    onMounted(() => {
      loadBrands();
    });
    let state = reactive({
      status: "",
      brands: [],
      products: [],
      selectedBrand: {},
      selectedItem: {},
      dialogStatus: "",
      itemSelected: false,
      qty: 0,
      cart: [],
    });
    const loadBrands = async () => {
      try {
        state.status = "loading brands...";
        const payload = await fetcher(`Brand`);
        if (payload.error === undefined) {
          state.brands = payload;
          state.status = `loaded ${state.brands.length} brands`;
        } else {
          state.status = payload.error;
        }
      } catch (err) {
        console.log(err);
        state.status = `Error has occured: ${err.message}`;
      }
    };
    const loadProducts = async () => {
      try {
        state.status = `finding products for Brand ${state.selectedBrand}...`;
        let payload = await fetcher(`Product/${state.selectedBrand}`);
        state.products = payload;
        state.status = `loaded ${state.products.length} Products`;
      } catch (err) {
        console.log(err);
        state.status = `Error has occured: ${err.message}`;
      }
      if (sessionStorage.getItem("cart")) {
        state.cart = JSON.parse(sessionStorage.getItem("cart"));
      }
    };
    const onRowSelect = (event) => {
      try {
        state.selectedItem = event.data;
        state.dialogStatus = "";
        state.itemSelected = true;
      } catch (err) {
        console.log(err);
        state.status = `Error has occured: ${err.message}`;
      }
    };
    const formatCurrency = (value) => {
      return value.toLocaleString("en-US", {
        style: "currency",
        currency: "USD",
      });
    };
    const addToCart = () => {
      const index = state.cart.findIndex(
        // is item already on the cart
        (item) => item.id === state.selectedItem.id
      );
      if (state.qty !== 0) {
        index === -1 // add
          ? state.cart.push({
              id: state.selectedItem.id,
              qty: state.qty,
              item: state.selectedItem,
            })
          : (state.cart[index] = {
              // replace
              id: state.selectedItem.id,
              qty: state.qty,
              item: state.selectedItem,
            });
        state.dialogStatus = `${state.qty} item(s) added`;
      } else {
        index === -1 ? null : state.cart.splice(index, 1); // remove
        state.dialogStatus = `item(s) removed`;
      }
      sessionStorage.setItem("cart", JSON.stringify(state.cart));
      state.qty = 0;
    };

    const viewCart = () => {
      router.push("cart");
    };

    return {
      state,
      loadProducts,
      onRowSelect,
      formatCurrency,
      addToCart,
      viewCart,
    };
  },
};
</script>