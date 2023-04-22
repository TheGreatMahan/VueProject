<template>
  <div class="container">
    <div class="heading">Your Orders</div>
    <div class="status">{{ state.status }}</div>
    <div id="order-list">
      <DataTable
        v-if="state.orders.length > 0"
        :value="state.orders"
        :scrollable="true"
        scrollHeight="60vh"
        dataKey="id"
        class="p-datatable-striped"
        v-model:selection="state.orderSelected"
        selectionMode="single"
        @row-select="onRowSelect"
      >
        <Column header="Order #" field="id" />
        <Column header="Date">
          <template #body="slotProps">
            {{ formatDate(slotProps.data.orderDate) }}
          </template>
        </Column>
      </DataTable>

      <Dialog v-model:visible="state.selectedAOrder" class="dialog-border">
        <div style="color: blue; font-size: larger; font-weight: bold">
          Order#{{ state.orderDetails[0].orderId }}
          <div style="margin-left: 35%; display: inline">
            {{ formatDate(state.orderDetails[0].orderDate) }}
          </div>
        </div>

        <br /><br /><br />

        <div
          style="
            font-weight: bold;
            margin-left: -8vw;
            text-align: center;
            font-size: larger;
          "
          class="order-head"
        >
          Quantities
        </div>
        <br />
        <br />

        <DataTable
          :value="state.orderDetails"
          :scrollable="true"
          scrollHeight="38vh"
          dataKey="id"
          class="p-datatable-striped"
        >
          <Column header="Name" field="name" />
          <Column header="S" field="qtyS" />
          <Column header="O" field="qtyO" />
          <Column header="B" field="qtyB" />
          <Column header="Extnd" field="sellingPrice" />
        </DataTable>
        <br />
        <br />
        <div>
          <div
            class="order-total-head"
            style="text-align: center; font-size: large"
          >
            <div style="font-weight: bold">Order Totals</div>
            <table style="border: solid; margin-top: 1vh; margin-left: 1vw">
              <tr>
                <td style="width: 20%; font-weight: bold">Sub:</td>
                <td style="width: 10%; text-align: right; padding-right: 3vw">
                  {{
                    formatCurrency(
                      state.orderDetails[0].sellingPrice +
                        state.orderDetails[0].sellingPrice
                    )
                  }}
                </td>
              </tr>
              <tr>
                <td style="width: 20%; font-weight: bold">Tax:</td>
                <td style="width: 10%; text-align: right; padding-right: 3vw">
                  {{
                    formatCurrency(
                      (state.orderDetails[0].sellingPrice +
                        state.orderDetails[0].sellingPrice) *
                        0.13
                    )
                  }}
                </td>
              </tr>
              <tr>
                <td style="width: 20%; font-weight: bold">Total:</td>
                <td style="width: 10%; text-align: right; padding-right: 3vw">
                  {{
                    formatCurrency(
                      (state.orderDetails[0].sellingPrice +
                        state.orderDetails[0].sellingPrice) *
                        1.13
                    )
                  }}
                </td>
              </tr>
            </table>
          </div>
        </div>
      </Dialog>
    </div>
  </div>
</template>

<script>
import { reactive, onMounted } from "vue";
import { fetcher } from "../util/apiutil";
export default {
  setup() {
    let state = reactive({
      status: "",
      dialogStatus: "",
      orders: [],
      selectedAOrder: false,
      orderSelected: {},
      orderDetails: [],
    });
    onMounted(() => {
      loadOrders();
    });
    const loadOrders = async () => {
      try {
        let customer = JSON.parse(sessionStorage.getItem("customer"));
        const payload = await fetcher(`order/${customer.email}`);
        if (payload.error === undefined) {
          state.orders = payload;
          state.status = `loaded ${state.orders.length} orders`;
          state.selectedAOrder = false;
        } else {
          state.status = payload.error;
        }
      } catch (err) {
        console.log(err);
        state.status = `Error has occured: ${err.message}`;
      }
    };
    const formatDate = (date) => {
      let d;
      // see if date is coming from server
      date === undefined ? (d = new Date()) : (d = new Date(Date.parse(date))); // from server
      let _day = d.getDate();
      let _month = d.getMonth() + 1;
      let _year = d.getFullYear();
      let _hour = d.getHours();
      let _min = d.getMinutes();
      if (_min < 10) {
        _min = "0" + _min;
      }
      return _year + "-" + _month + "-" + _day + " " + _hour + ":" + _min;
    };

    const onRowSelect = async (event) => {
      try {
        let customer = JSON.parse(sessionStorage.getItem("customer"));
        state.orderSelected = event.data;
        const payload = await fetcher(
          `order/${state.orderSelected.id}/${customer.email}`
        );
        state.orderDetails = payload;
        state.dialogStatus = `details for order ${state.orderSelected.id}`;
        state.selectedAOrder = true;
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

    return {
      state,
      formatDate,
      onRowSelect,
      formatCurrency,
    };
  },
};
</script>