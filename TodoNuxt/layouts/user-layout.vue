<template>
  <v-layout class="rounded rounded-md">
    <v-app-bar title="Todo Application" class="bg-green">
      <template v-slot:append v-if="user">
        <span class="mr-5">Hi, {{ user.name }}</span>
      </template>
    </v-app-bar>

    <v-navigation-drawer>
      <v-list base-color="green">
        <v-list-item prepend-icon="mdi-home" to="/admin">
          Dashboard
        </v-list-item>
        <v-list-item prepend-icon="mdi-check-circle" to="/admin/task-done">
          Task Done
        </v-list-item>
        <v-list-item prepend-icon="mdi-close-circle" to="/admin/task-canceled">
          Task Canceled
        </v-list-item>
        <v-list-item prepend-icon="mdi-logout" @click="logout">
          Logout
        </v-list-item>
      </v-list>
    </v-navigation-drawer>

    <v-main class="mt-5" style="width: 100%">
      <NuxtPage />
    </v-main>
  </v-layout>
</template>
<script setup>
import { onMounted, ref } from "vue";
import { useRouter } from "vue-router";
import Cookies from "js-cookie";

const router = useRouter();
const user = ref(null);
onMounted(() => {
  const token = Cookies.get("token");
  const getUser = localStorage.getItem("user");
  if (getUser) {
    user.value = JSON.parse(getUser);
  }
  if (!token) router.push("/");
});

const logout = () => {
  Cookies.remove("token");
  router.push("/");
};
</script>
