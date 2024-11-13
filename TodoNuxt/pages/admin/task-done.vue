<template>
  <div class="pa-5">
    <div v-if="todos.length === 0">
      <div class="text-center">You have not completed any tasks.</div>
    </div>
    <div v-else>
      <v-row>
        <v-col>
          <div class="px-3 text-h6 font-weight-bold">Task Done</div>
          <div class="px-3 text-caption text-grey">
            All task completed are shown here
          </div>
        </v-col>
      </v-row>
      <v-row>
        <v-col v-for="(todo, index) in todos" :key="index" cols="3">
          <TodoCardComponent :todo="todo" />
        </v-col>
      </v-row>
    </div>
    <InnerLoading ref="loading" />
    <NotifyComponent ref="notify" :configuration="notifyConfig" />
  </div>
</template>
<script setup>
import { onMounted } from "vue";
import { getTodoByUserId } from "../../libraries/api.ts";

const [loading, notify, todos, user] = [
  ref(null),
  ref(null),
  ref([]),
  ref(null),
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
      todos.value = response.data.filter((data) => data.status === 1);
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

definePageMeta({
  layout: "user-layout",
});
</script>
