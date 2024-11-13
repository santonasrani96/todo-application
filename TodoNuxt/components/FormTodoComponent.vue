<template>
  <v-dialog v-model="dialog" max-width="500">
    <v-card>
      <v-card-title>
        {{ isUpdate ? "Update task" : "Create a new Task" }}
      </v-card-title>
      <v-card-text>
        <v-text-field
          v-model="form.subject"
          dense
          label="Subject"
          variant="outlined"
        ></v-text-field>
        <v-textarea
          v-model="form.description"
          dense
          label="Description"
          variant="outlined"
        ></v-textarea>
        <v-btn
          class="text-none"
          block
          color="green"
          @click="isUpdate ? update() : submit()"
          >{{ isUpdate ? "Update" : "Submit" }}</v-btn
        >
      </v-card-text>
    </v-card>
  </v-dialog>
</template>
<script setup>
import { ref, computed } from "vue";
import { formInvalid } from "../libraries/validations.ts";

const _emits = defineEmits(["submit", "update"]);
const [dialog, form, isPassword, loadingBtn, isUpdate, todoSelected] = [
  ref(false),
  ref({
    subject: "",
    description: "",
  }),
  ref(true),
  ref(false),
  ref(false),
  ref(null),
];

const formMandatory = computed(() => {
  return {
    subject: form.value.subject,
    description: form.value.description,
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

const update = () => {
  const params = {
    todoId: todoSelected.value.id,
    subject: form.value.subject,
    description: form.value.description,
  };

  _emits("update", params);
};

const submit = () => {
  const params = {
    subject: form.value.subject,
    description: form.value.description,
  };

  _emits("submit", params);
};

const passData = (item) => {
  isUpdate.value = true;
  todoSelected.value = item;
  form.value.subject = item.subject;
  form.value.description = item.description;
};

const emptyForm = () => {
  isUpdate.value = false;
  todoSelected.value = null;
  form.value.description = "";
  form.value.subject = "";
};

defineExpose({
  show,
  hide,
  emptyForm,
  showLoading,
  hideLoading,
  passData,
});
</script>
