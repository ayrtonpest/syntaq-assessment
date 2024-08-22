<template>
  <div class="about">
    <router-link :to="{ name: 'Invoices' }">Back</router-link>

    <h2>Invoice Details</h2>

    <span>Invoice #{{$route.params.id}}</span>

    <h3>Line Items</h3>

    <table>
      <thead>
        <th>ID</th>
        <th>Description</th>
        <th>Quantity</th>
        <th>Cost</th>
        <th>Billable</th>
        <th>Actions</th>
      </thead>
      <tbody>
        <tr v-for="item in state.lineItems.lineItem" :key="item.id">
          <td>{{item.id}}</td>
          <td>{{item.description}}</td>
          <td>{{item.quantity}}</td>
          <td>{{item.cost}}</td>
          <td>
            <input type="checkbox" :id="item.id" @change="handleBillableStatus($event)" :name="item.invoiceId" :value="item.isBillable" :checked="item.isBillable ? true : false">
          </td>
          <td>
            <button @click="deleteLineItem(item.id)">Delete</button>
            <button @click="editLineItem(item)">Edit</button>
          </td>
        </tr>
      </tbody>
    </table>

    <div class="text-right">
      <strong>Total Value: </strong>{{state.lineItems.grandTotal}}
    </div>
    <div class="text-right">
      <strong>Total Billable Value: </strong>{{state.lineItems.totalBillableValue}}
    </div>

    <form @submit.prevent>
      <h4>{{state.isEditing ? 'Edit Line Item' : 'Create Line Item'}}</h4>
      <input type="text" name="description" placeholder="Description" v-model="state.description" />
      <input type="number" name="quantity" placeholder="Quantity" v-model="state.quantity" />
      <input type="number" name="cost" placeholder="Cost" v-model="state.cost" />
      Billable: <input type="checkbox" name="isbillable" v-model="state.isbillable" />
      <br />
      <br />
      <button @click="state.isEditing ? updateLineItem() : createLineItem()">
        {{state.isEditing ? 'Update Item' : 'Add Item'}}
      </button>
      <button v-if="state.isEditing" @click="cancelEdit">Cancel</button>
    </form>
  </div>
  <snackbar :message="state.snackbarMessage" :visible="state.showSnackbar"/>
</template>


<script lang="ts">
import snackbar from "@/components/snackbar.vue";
import { defineComponent, onMounted, reactive } from "vue";

export default defineComponent({
  name: "Invoice",
  props: {
    id: {
      type: [String, Number],
      required: true
    }
  },
  components: {
    snackbar 
  },
  methods: {
    handleBillableStatus(event: any) {
      let { value, id, name } = event.target;
      let newValue = value === "true" ? false : true;

      fetch(`http://localhost:5000/invoices/Update/`, {
        method: "PUT",
        headers: {
          "Content-Type": "application/json"
        },
        body: JSON.stringify({
          lineItemId: parseInt(id),
          isBillable: newValue,
          invoiceId: parseInt(name)
        })
      }).then(() => {
        window.location.reload();
      });
    },

  },
  
  setup(props) {
    const state = reactive({
      id: 0,
      lineItems: [],
      description: "",
      quantity: "0",
      cost: "0",
      isbillable: true,
      invoiceId: props.id,
      editingLineItemId: null,
      isEditing: false,
      showSnackbar: false,
      snackbarMessage: "",
    });

    function editLineItem(item: any) {
      state.description = item.description;
      state.quantity = item.quantity.toString();
      state.cost = item.cost.toString();
      state.isbillable = item.isBillable;
      state.editingLineItemId = item.id;
      state.isEditing = true;
    }

    function cancelEdit() {
      state.description = "";
      state.quantity = "0";
      state.cost = "0";
      state.isbillable = true;
      state.editingLineItemId = null;
      state.isEditing = false;
    }

    function fetchLineItems() {
      fetch(`http://localhost:5000/invoices/${props.id}`, {
        method: "GET",
        headers: {
          "Content-Type": "application/json"
        }
      }).then((response) => {
        response.json().then(lineItems => (state.lineItems = lineItems));
      });
    }

    function createLineItem() {
      fetch(`http://localhost:5000/invoices/${props.id}`, {
        method: "POST",
        headers: {
          "Content-Type": "application/json"
        },
        body: JSON.stringify({
          description: state.description,
          quantity: Number(state.quantity),
          cost: Number(state.cost),
          isBillable: state.isbillable
        })
      }).then(() => {
        fetchLineItems();
        showSnackbar(`Line item has been created successfully`);
      });
    }

    function deleteLineItem(lineItemId: number) {
      fetch(`http://localhost:5000/invoices/line-item/${lineItemId}`, {
        method: "DELETE",
        headers: {
          "Content-Type": "application/json"
        }
      }).then(() => {
        fetchLineItems();
        showSnackbar(`Line item has been deleted successfully`);
      });
    }

    function updateLineItem() {
      fetch(`http://localhost:5000/invoices/${props.id}`, {
        method: "POST",
        headers: {
          "Content-Type": "application/json"
        },
        body: JSON.stringify({
          id: state.editingLineItemId,
          description: state.description,
          quantity: Number(state.quantity),
          cost: Number(state.cost),
          isBillable: state.isbillable,
        })
      }).then(() => {
        cancelEdit();
        fetchLineItems();
        showSnackbar(`Line item has been updated successfully`);
      });
    }

    function showSnackbar(message: string) {
      state.snackbarMessage = message;
      state.showSnackbar = true;
      console.log(state.snackbarMessage, state.showSnackbar)
      setTimeout(() => {
        state.showSnackbar = false;
      }, 3000); 
    }

    onMounted(fetchLineItems);

    return {
      state,
      createLineItem,
      deleteLineItem,
      fetchLineItems,
      editLineItem,
      cancelEdit,
      updateLineItem,
      showSnackbar,
    };
  }
});
</script>
