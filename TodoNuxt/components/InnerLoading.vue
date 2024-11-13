<template>
  <v-overlay v-model="isOverlay">
    <v-progress-circular
      v-if="isLoading"
      indeterminate
      color="green"
      :size="50"
    ></v-progress-circular>
  </v-overlay>
</template>
<script setup>
import { ref } from "vue";

const isOverlay = ref(false);
const isLoading = ref(false);

const show = () => {
  isOverlay.value = true;
  isLoading.value = true;
};

const hide = () => {
  isOverlay.value = false;
  isLoading.value = false;
};

defineExpose({
  show,
  hide,
});
</script>
