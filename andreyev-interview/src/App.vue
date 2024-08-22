<template>
  <div>
    <h1>Invoicing Application</h1>
    
    <div class="tabs">
      <router-link to="/clients" class="tab" :class="{ active: $route.name === 'Clients' }">Clients</router-link>
      <router-link to="/" class="tab" :class="{ active: $route.name === 'Invoices' }">Invoices</router-link>
    </div>

    <router-view/>
  </div>
  <Snackbar :message="snackbarMessage" :visible="snackbarVisible" />
</template>

<script lang="ts">
import { defineComponent, reactive } from 'vue';
import Snackbar from './components/snackbar.vue';
import { provide } from 'vue'

export default defineComponent({
  name: 'App',
  components: {
    Snackbar
  },
  setup() {
    const state = reactive({
      snackbarMessage: '',
      snackbarVisible: false
    });

    function showSnackbar(message: string) {
      state.snackbarMessage = message;
      state.snackbarVisible = true;
      setTimeout(() => {
        state.snackbarVisible = false;
      }, 3000);
    }

    provide('showSnackbar', showSnackbar);

    return {
      snackbarMessage: state.snackbarMessage,
      snackbarVisible: state.snackbarVisible,
      showSnackbar
    };
  },

});
</script>

<style>
  .tabs {
    display: flex;
    justify-content: center;
    margin-bottom: 20px;
  }

  .tab {
    padding: 10px 20px;
    text-decoration: none;
    color: #333;
    border-bottom: 2px solid transparent;
  }

  .tab.active {
    border-bottom: 2px solid #333;
    font-weight: bold;
  }

  .tab:hover {
    color: #666;
  }
</style>
