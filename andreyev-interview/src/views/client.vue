<template>
    <div class="about">
        <router-link :to="{ name: 'Clients' }">Back</router-link>

        <h2>Client Details</h2>

        <span>Client #{{$route.params.id}}</span>

        <form @submit.prevent="updateClient">
            <h4>Edit Client Name</h4>
            <input type="text" name="name" placeholder="Client Name" v-model="state.name" />
            <br />
            <br />
            <button type="submit">Update Client</button>
            <button type="button" @click="cancelEdit">Cancel</button>
        </form>
    </div>
    <snackbar :message="state.snackbarMessage" :visible="state.showSnackbar"/>
</template>
  
<script lang="ts">
  import { defineComponent, onMounted, reactive } from "vue";
  import snackbar from '@/components/snackbar.vue';

  export default defineComponent({
    name: "Client",
    components: {
        snackbar 
    },
    props: {
      id: {
        type: [String, Number],
        required: true
      }
    },
    
    setup(props) {
      const state = reactive({
        name: "",
        originalName: "",
        clientId: props.id,
        showSnackbar: false,
        snackbarMessage: ""
      });
  
      function fetchClientDetails() {
        fetch(`http://localhost:5000/clients/${props.id}`, {
          method: "GET",
          headers: {
            "Content-Type": "application/json"
          }
        }).then((response) => {
          response.json().then(client => {
            state.name = client.name;
            state.originalName = client.name;
          });
        });
      }
  
      function updateClient() {
        fetch(`http://localhost:5000/clients/${props.id}`, {
          method: "PUT",
          headers: {
            "Content-Type": "application/json"
          },
          body: JSON.stringify({
            name: state.name
          })
        }).then((response) => {
          if (response.ok) {
            if (state.name !== state.originalName) {
              showSnackbar(`Client #${state.clientId} name updated from ${state.originalName} to ${state.name}`);
            }
            setTimeout(() => {
              window.location.href = '/clients'; 
            }, 3000); 
          } else {
            response.text().then((text) => {
              alert(`Failed to update client: ${text}`);
            });
          }
        });
      }
  
      function cancelEdit() {
        window.location.href = '/clients'; 
      }

      function showSnackbar(message: string) {
        state.snackbarMessage = message;
        state.showSnackbar = true;
        setTimeout(() => {
          state.showSnackbar = false;
        }, 3000);
      }
  
      onMounted(fetchClientDetails);
  
      return {
        state,
        updateClient,
        cancelEdit
      };
    }
  });
  </script>
  