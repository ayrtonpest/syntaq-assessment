<template>
  <div class="home">
    <form @submit.prevent>
      <label for="invoices">Create a new invoice</label>
      <input type="text" name="invoices" v-model="state.description" placeholder="Description" />

      <label for="client">Attach to Client</label>
      <select v-if="state.clients.length >= 1" v-model="state.selectedClientId">
        <option v-for="client in state.clients" :key="client.id" :value="client.id">
          {{ client.name }}
        </option>
      </select>

      <label for="discount">Discount (%)</label>
      <input type="number" name="discount" v-model="state.discount" placeholder="Discount" min="0" max="100" />

      <button @click="createInvoice">Create Invoice</button>
    </form>

    <hr />

    <table>
      <thead>
        <tr>
          <th>ID</th>
          <th>Description</th>
          <th>Client</th>
          <th>Invoice Link</th>
          <th>Total value</th>
          <th>Total Billable Amount</th>
          <th>Discount (%)</th>
          <th>Actions</th>
        </tr>
      </thead>
      <tbody>
        <tr v-for="invoice in state.invoices" :key="invoice.id">
          <td>{{ invoice.id }}</td>
          <td>{{ invoice.description }}</td>
          <td>{{ invoice.client ? invoice.client.name : 'No Client' }}</td>
          <td>
            <router-link :to="{ name: 'Invoice', params: { id: invoice.id }}">
              Open
            </router-link>
          </td>
          <td>{{ invoice.totalValue }}</td>
          <td>{{ invoice.totalBillableValue }}</td>
          <td>{{ invoice.discount }}</td>
          <td>
            <button @click="deleteInvoice(invoice.id)">Delete</button>
          </td>
        </tr>
      </tbody>
    </table>
  </div>
  <snackbar :message="state.snackbarMessage" :visible="state.showSnackbar"/>
</template>

<script lang="ts">
import snackbar from '@/components/snackbar.vue';
import { defineComponent, onMounted, reactive } from 'vue';

export default defineComponent({
  name: 'Invoices',
  components: {
    snackbar 
  },
  setup() {
    const state = reactive({
      invoices: [],
      clients: [],  
      description: "",
      selectedClientId: null, 
      discount: 0 as number,
      showSnackbar: false,
      snackbarMessage: ""
    });

    function fetchInvoices() {
      fetch("http://localhost:5000/invoices", {
        method: "GET",
        headers: {
          "Content-Type": "application/json"
        },
      }).then((response) => {
        response.json().then(invoices => (state.invoices = invoices.invoices))
      })
    }

    function fetchClients() {
      fetch("http://localhost:5000/clients", {
        method: "GET",
        headers: {
          "Content-Type": "application/json"
        },
      }).then((response) => {
        response.json().then(clientModel => (state.clients = clientModel.clients))
      })
    }

    function createInvoice() {
      fetch("http://localhost:5000/invoices", {
        method: "PUT",
        headers: {
          "Content-Type": "application/json"
        },
        body: JSON.stringify({
          description: state.description,
          clientId: state.selectedClientId,
          discount: state.discount, 
        })
      }).then((response) => response.json())
        .then(data => {
          fetchInvoices();
          showSnackbar(`Invoice #${data.id} created successfully`);
        });
    }

    function deleteInvoice(invoiceId: number) {
      fetch(`http://localhost:5000/invoices/${invoiceId}`, {
        method: "DELETE",
        headers: {
          "Content-Type": "application/json"
        },
      }).then(() => {
        fetchInvoices();
        showSnackbar(`Invoice #${invoiceId} deleted successfully`);
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

    onMounted(() => {
      fetchInvoices();
      fetchClients();
    });

    return { state, createInvoice, deleteInvoice };
  }
});
</script>
