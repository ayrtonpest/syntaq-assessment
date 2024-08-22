<!-- src/components/Snackbar.vue -->
<template>
  <div v-if="visible" class="snackbar">
    {{ message }}
  </div>
</template>

<script lang="ts">
import { defineComponent } from 'vue';

export default defineComponent({
  name: 'Snackbar',
  props: {
    message: {
      type: String,
      required: true
    },
    visible: {
      type: Boolean,
      required: true
    }
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
</style>
