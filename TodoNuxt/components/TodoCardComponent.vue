<template>
  <v-card :class="isSelected(_props.todo.id) ? 'bg-green' : ''">
    <v-card-title>{{ _props.todo.subject }}</v-card-title>
    <v-card-subtitle>
      <v-chip :color="chipConfig.color" variant="flat" size="x-small">
        {{ chipConfig.label }}
      </v-chip>
    </v-card-subtitle>
    <v-card-text>
      {{ _props.todo.description }}

      <div class="mt-5">
        <v-btn
          color="green"
          class="text-none"
          variant="flat"
          block
          @click="updateTodo(_props.todo)"
          >Update</v-btn
        >
        <v-btn
          color="green"
          class="text-none mt-2"
          variant="flat"
          block
          @click="selectTodo(_props.todo)"
          >{{
            isSelected(_props.todo.id) ? "Tugas dipilih" : "Pilih Tugas"
          }}</v-btn
        >
      </div>
    </v-card-text>
  </v-card>
</template>
<script setup>
import { computed, ref, reactive } from "vue";

const _emits = defineEmits(["selected", "update"]);
const _props = defineProps({
  todo: {
    type: Object,
    default: null,
  },
});

const [selectedTodos] = [ref([])];
const state = reactive({
  selectedTodos1: [],
});

const chipConfig = computed(() => {
  if (!_props.todo.status) {
    return {
      label: "Tugas belum ditandai",
      color: "secondary",
    };
  } else if (_props.todo.status === 0) {
    return {
      label: "Tugas dibatalkan",
      color: "red",
    };
  } else if (_props.todo.status === 1) {
    return {
      label: "Tugas selesai",
      color: "green",
    };
  }
});

const isSelected = (todoId) => {
  if (selectedTodos.value.length === 0) {
    return false;
  }

  const todoSelected = selectedTodos.value.find((todo) => todo.id === todoId);

  if (todoSelected) {
    return true;
  }
};

const selectTodo = (todo) => {
  const todoIndex = selectedTodos.value.findIndex((t) => t.id === todo.id);
  if (todoIndex > -1) {
    selectedTodos.value.splice(todoIndex, 1);
  } else {
    selectedTodos.value.push(todo);
  }

  console.log(selectedTodos.value.length);

  _emits("selected", selectedTodos.value);
};

const { selectedTodos1 } = toRefs(state);

const updateTodo = (todo) => {
  _emits("update", todo);
};

// const insertTodo = (todos, todo) => {

// }
</script>
