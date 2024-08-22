<template>
    <div class="home">
        <form @submit.prevent>
            <label for="clients">Create a new client</label>
            <input type="text" name="clients" v-model="state.name" placeholder="Client Name" />
            <button @click="createClient">Create Client</button>
        </form>

        <hr />

        <table>
        <thead>
            <tr>
            <th>ID</th>
            <th>Name</th>
            <th>Actions</th>
            </tr>
        </thead>
        <tbody>
            <tr v-for="client in state.clients" :key="client.id">
            <td>{{ client.id }}</td>
            <td>{{ client.name }}</td>
            <td>
                <router-link :to="{ name: 'Client', params: { id: client.id } }">
                <button>Edit</button>
                </router-link>
                <button @click="deleteClient(client.id)">Delete</button>
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
    name: 'Clients',
    components: {
        snackbar 
    },
    setup() {
      const state = reactive({
        clients: [],
        name: "",
        showSnackbar: false,
        snackbarMessage: ""
      });
  
      function fetchClients() {
        fetch("http://localhost:5000/clients", {
          method: "GET",
          headers: {
            "Content-Type": "application/json"
          },
        }).then((response) => {
          response.json().then(clients => (state.clients = clients.clients))
        })
      }
  
      function createClient() {
        fetch("http://localhost:5000/clients", {
          method: "POST",
          headers: {
            "Content-Type": "application/json"
          },
          body: JSON.stringify({
            name: state.name
          })
        }).then((response) => response.json())
          .then(data => {
            fetchClients();
            showSnackbar(`Client #${data.id} created successfully`);
          });
      }
  
      function deleteClient(clientId: number) {
        fetch(`http://localhost:5000/clients/${clientId}`, {
          method: "DELETE",
          headers: {
            "Content-Type": "application/json"
          },
        }).then(() => {
          fetchClients();
          showSnackbar(`Client #${clientId} deleted successfully`);
        });
      }
  
      function showSnackbar(message: string) {
        state.snackbarMessage = message;
        state.showSnackbar = true;
        setTimeout(() => {
          state.showSnackbar = false;
        }, 3000);
      }
  
      onMounted(fetchClients);
  
      return { state, createClient, deleteClient };
    }
  });
  </script>
  
  <style scoped>
    .snackbar {
        transition: opacity 0.5s ease-in-out;
        opacity: 1;
        min-width: 250px;
        background-color: #333;
        color: #fff;
        text-align: center;
        border-radius: 2px;
        padding: 16px;
        position: fixed;
        z-index: 1;
        right: 30px;
        bottom: 30px;
        font-size: 17px;
    }
    
    .snackbar.hide {
        opacity: 0;
    }
  </style>
  