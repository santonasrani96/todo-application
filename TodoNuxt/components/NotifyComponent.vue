<template>
  <v-snackbar
    v-model="snackbar"
    :timeout="2000"
    :color="_props.configuration.color"
    elevation="24"
  >
    <div class="text-center">{{ _props.configuration.message }}</div>
  </v-snackbar>
</template>
<script setup>
import { ref } from "vue";
const _props = defineProps({
  configuration: {
    type: Object,
    default: null,
  },
});

const snackbar = ref(false);

const show = () => {
  snackbar.value = true;
};

const hide = () => {
  snackbar.value = false;
};

defineExpose({
  show,
  hide,
});
</script>
