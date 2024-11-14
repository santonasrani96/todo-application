<template>
  <v-card :class="_props.selectedTodo ? 'bg-green' : ''">
    <v-card-title>{{ _props.todo.subject }}</v-card-title>
    <v-card-subtitle>
      <v-chip :color="chipConfig.color" variant="flat" size="x-small">
        {{ chipConfig.label }}
      </v-chip>
    </v-card-subtitle>
    <v-card-text>
      {{ _props.todo.description }}

      <div class="mt-5 text-center" v-if="_props.todo.status === null">
        <v-row>
          <v-col>
            <v-btn
              color="info"
              size="24"
              variant="flat"
              density="compact"
              @click="updateTodo(_props.todo)"
              ><v-icon icon="mdi-pencil" color="white"></v-icon
            ></v-btn>
          </v-col>
          <v-col>
            <v-btn
              color="green"
              size="24"
              variant="flat"
              density="compact"
              @click="selectTodo(_props.todo)"
              ><v-icon icon="mdi-target" color="white"></v-icon
            ></v-btn>
          </v-col>
          <v-col>
            <v-btn
              color="red"
              size="24"
              variant="flat"
              density="compact"
              @click="deleteTodo(_props.todo)"
              ><v-icon icon="mdi-delete" color="white"></v-icon
            ></v-btn>
          </v-col>
        </v-row>
      </div>
    </v-card-text>
  </v-card>
</template>
<script setup>
import { computed, ref } from "vue";

const _emits = defineEmits(["selected", "update", "delete"]);
const _props = defineProps({
  todo: {
    type: Object,
    default: null,
  },
  selectedTodo: {
    type: Object,
    default: null,
  },
});

const [selectedTodos] = [ref([])];

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

const selectTodo = (todo) => {
  _emits("selected", todo);
};

const updateTodo = (todo) => {
  _emits("update", todo);
};

const deleteTodo = (todo) => {
  _emits("delete", todo);
};
</script>
