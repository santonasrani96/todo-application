<template>
  <v-dialog v-model="dialog" max-width="500">
    <v-card>
      <v-card-title>Register User</v-card-title>
      <v-card-text>
        <v-text-field
          v-model="form.name"
          label="Full Name"
          variant="outlined"
        />
        <v-text-field
          v-model="form.userId"
          label="User ID"
          variant="outlined"
        />
        <v-text-field
          v-model="form.password"
          :append-inner-icon="!isPassword ? 'mdi-eye' : 'mdi-eye-off'"
          :type="!isPassword ? 'text' : 'password'"
          label="Password"
          variant="outlined"
          @click:append-inner="isPassword = !isPassword"
        ></v-text-field>
      </v-card-text>
      <v-card-actions>
        <v-btn
          :loading="loadingBtn"
          :disabled="loadingBtn || formInvalid(formMandatory)"
          :class="
            formInvalid(formMandatory)
              ? 'text-none text-blue-grey-darken-3'
              : 'text-none'
          "
          :color="
            formInvalid(formMandatory)
              ? 'blue-grey-lighten-5'
              : 'indigo-darken-3'
          "
          variant="flat"
          block
          @click="registerUser"
        >
          Register
        </v-btn>
      </v-card-actions>
      <template v-slot:actions>
        <v-btn
          block
          variant="outlined"
          class="ml-auto text-none"
          text="Close"
          @click="hide()"
        ></v-btn>
      </template>
    </v-card>
  </v-dialog>
</template>
<script setup>
import { ref, computed } from "vue";
import { formInvalid } from "../libraries/validations.ts";

const _emits = defineEmits(["submit"]);
const [dialog, form, isPassword, loadingBtn] = [
  ref(false),
  ref({
    name: "",
    userId: "",
    password: "",
  }),
  ref(true),
  ref(false),
];

const formMandatory = computed(() => {
  return {
    name: form.value.name,
    userId: form.value.userId,
    password: form.value.password,
  };
});

const show = () => {
  dialog.value = true;
};

const showLoading = () => {
  loadingBtn.value = true;
};

const hide = () => {
  dialog.value = false;
};

const hideLoading = () => {
  loadingBtn.value = false;
};

const registerUser = () => {
  const params = {
    name: form.value.name,
    userId: form.value.userId,
    password: form.value.password,
  };

  _emits("submit", params);
};

const emptyForm = () => {
  form.value.name = "";
  form.value.userId = "";
  form.value.password = "";
};

defineExpose({
  show,
  hide,
  emptyForm,
  showLoading,
  hideLoading,
});
</script>
