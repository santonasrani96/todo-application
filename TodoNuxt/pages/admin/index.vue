<template>
  <div>
    <div class="text-center">
      <div v-if="loading">
        <v-btn class="text-none mt-3" color="green" @click="showFormTodo"
          >Create a new Task {{ selectedTodos.length }}</v-btn
        >
      </div>
      <div v-if="todos.length === 0">You do not have any active todos</div>
    </div>
    <div class="mt-5 pa-3" v-if="todos.length > 0">
      <v-row>
        <v-col>
          <div class="float-right" v-if="selectedTodos.length > 0">
            <v-btn
              prepend-icon="mdi-check-all"
              class="text-none mr-3"
              color="green"
              @click="doneMultipleTodo"
            >
              <template v-slot:prepend>
                <v-icon color="white"></v-icon> </template
              >Done</v-btn
            >
            <v-btn
              prepend-icon="mdi-close-box"
              class="text-none"
              color="red"
              @click="cancelMultipleTodo"
              ><template v-slot:prepend>
                <v-icon color="white"></v-icon> </template
              >Canceled</v-btn
            >
          </div>
        </v-col>
      </v-row>
      <v-row>
        <v-col>
          <div class="px-3 text-h6 font-weight-bold">New Task</div>
          <div class="px-3 text-caption text-grey">Task unmarked here</div>
        </v-col>
      </v-row>
      <v-row>
        <v-col v-for="(todo, index) in todos" :key="index" cols="3">
          <TodoCardComponent
            :todo="todo"
            @selected="onSelected"
            @update="onUpdate"
          />
        </v-col>
      </v-row>
    </div>
    <NotifyComponent ref="notify" :configuration="notifyConfig" />
    <InnerLoading ref="loading" />
    <FormTodoComponent ref="form" @submit="onSubmit" @update="onUpdateTodo" />
  </div>
</template>
<script setup>
import { ref, onMounted } from "vue";
import {
  getTodoByUserId,
  createTodo,
  updateTodo,
  updateBatchStatusTodo,
} from "../../libraries/api.ts";

const [todos, user, notify, loading, form, selectedTodos] = [
  ref([]),
  ref(null),
  ref(null),
  ref(null),
  ref(null),
  ref([]),
];

// notifyConfig
const notifyConfig = ref({
  message: "",
  color: "",
});
// end notifyConfig

onMounted(() => {
  getUserData();
  init();
});

const getUserData = () => {
  const getUser = localStorage.getItem("user");
  if (getUser) {
    user.value = JSON.parse(getUser);
  }
};

const init = () => {
  if (!user.value) {
    notifyConfig.value.message = "User not found";
    notifyConfig.value.color = "red-darken-3";

    notify.value.show();
    return;
  }

  loading.value.show();
  getTodoByUserId(user.value.id)
    .then((response) => {
      todos.value = response.data.filter((data) => !data.status);
      if (todos.value.length === 0) {
        notifyConfig.value.message = "You do not have any active todo";
        notifyConfig.value.color = "success";

        notify.value.show();
      }
    })
    .catch((err) => {
      console.log("Failed to call API getTodoByUserId() ", err);
      notifyConfig.value.message = "Failed to retrieve data";
      notifyConfig.value.color = "red-darken-3";

      notify.value.show();
    })
    .finally(() => {
      loading.value.hide();
    });
};

const onSelected = (todoSelected) => {
  if (todoSelected && todoSelected.length > 0) {
    selectedTodos.value = todoSelected;
  }
};

const onUpdate = (todo) => {
  form.value.passData(todo);
  form.value.show();
};

const doneMultipleTodo = () => {
  loading.value.show();
  const params = {
    ids: selectedTodos.value.map((todo) => todo.id),
    status: 1,
  };

  updateBatchStatusTodo(params)
    .then((response) => {
      notifyConfig.value.message = "Todo has been succesfully to update";
      notifyConfig.value.color = "success";

      init();
    })
    .catch((err) => {
      console.log("Failed to call API updateBatchStatusTodo() ", err);
      notifyConfig.value.message = "Failed to update data";
      notifyConfig.value.color = "red-darken-3";
    })
    .finally(() => {
      notify.value.show();
      loading.value.hide();
    });
};

const cancelMultipleTodo = () => {
  loading.value.show();
  const params = {
    ids: selectedTodos.value.map((todo) => todo.id),
    status: 0,
  };

  updateBatchStatusTodo(params)
    .then((response) => {
      notifyConfig.value.message = "Todo has been succesfully to update";
      notifyConfig.value.color = "success";

      init();
    })
    .catch((err) => {
      console.log("Failed to call API updateBatchStatusTodo() ", err);
      notifyConfig.value.message = "Failed to update data";
      notifyConfig.value.color = "red-darken-3";
    })
    .finally(() => {
      notify.value.show();
      loading.value.hide();
    });
};

const showFormTodo = () => {
  form.value.show();
};

const onSubmit = (params) => {
  const createParams = {
    userId: user.value.id,
    subject: params.subject,
    description: params.description,
    status: null,
  };

  form.value.showLoading();
  createTodo(createParams)
    .then((response) => {
      notifyConfig.value.message = "Task successfully created";
      notifyConfig.value.color = "success";

      form.value.emptyForm();
      form.value.hide();
      init();
    })
    .catch((err) => {
      console.log("Failed to call API createTodo() ", err);
      notifyConfig.value.message = "Failed to create a new task";
      notifyConfig.value.color = "red-darken-3";
    })
    .finally(() => {
      notify.value.show();
      form.value.hideLoading();
    });
};

const onUpdateTodo = (params) => {
  const updateParams = {
    userId: user.value.id,
    subject: params.subject,
    description: params.description,
    status: null,
  };

  form.value.showLoading();
  updateTodo(params.todoId, updateParams)
    .then((response) => {
      notifyConfig.value.message = "Task successfully updated";
      notifyConfig.value.color = "success";

      form.value.emptyForm();
      form.value.hide();
      init();
    })
    .catch((err) => {
      console.log("Failed to call API updateTodo() ", err);
      notifyConfig.value.message = "Failed to update a new task";
      notifyConfig.value.color = "red-darken-3";
    })
    .finally(() => {
      notify.value.show();
      form.value.hideLoading();
    });
};

definePageMeta({
  layout: "user-layout",
});
</script>
