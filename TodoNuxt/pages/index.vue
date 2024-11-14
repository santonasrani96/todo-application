<template>
  <v-container fill-height>
    <v-row justify="center" align="center" style="min-height: 100vh">
      <v-col cols="auto">
        <v-card style="width: 500px">
          <v-card-title class="text-center pa-5">
            Todo Application
          </v-card-title>
          <v-card-text class="mt-5 pa-5">
            <v-text-field v-model="form.userId" label="User ID"></v-text-field>
            <v-text-field
              v-model="form.password"
              :append-inner-icon="!isPassword ? 'mdi-eye' : 'mdi-eye-off'"
              :type="!isPassword ? 'text' : 'password'"
              label="Password"
              @click:append-inner="isPassword = !isPassword"
            ></v-text-field>
            <v-btn
              class="text-none mb-4"
              color="indigo-darken-3"
              size="x-large"
              variant="flat"
              block
              :loading="loadingBtn"
              @click="loginUser"
            >
              Login
            </v-btn>
            <v-btn
              class="no-uppercase"
              block
              variant="text"
              color="primary"
              @click="showFormRegister"
            >
              Don't have account?
            </v-btn>
          </v-card-text>
        </v-card>
      </v-col>
    </v-row>
    <FormRegisterComponent ref="formRegister" @submit="onSubmit" />
    <NotifyComponent ref="notify" :configuration="notifyConfig" />
  </v-container>
</template>
<script setup>
import { ref, computed, onMounted } from "vue";
import { useRouter } from "vue-router";
import Cookies from "js-cookie";
import { createUser, login } from "../libraries/api.ts";

const router = useRouter();
const [isPassword, form, formRegister, notify, loadingBtn] = [
  ref(true),
  ref({
    userId: null,
    password: null,
  }),
  ref(null),
  ref(null),
  ref(false),
];

// notifyConfig
const notifyConfig = ref({
  message: "",
  color: "",
});
// end notifyConfig

onMounted(() => {
  // check if user has been logged
  checkUserLogin();
});

const checkUserLogin = () => {
  const token = Cookies.get("token");
  if (token) router.push("/admin/");
};

const loginUser = () => {
  loadingBtn.value = true;
  const params = {
    userId: form.value.userId,
    password: form.value.password,
  };

  login(params)
    .then((response) => {
      const token = response.data.token;
      const user = response.data.user;
      Cookies.set("token", token, { expires: 1 });
      localStorage.setItem("user", JSON.stringify(user));

      router.push("/admin/");
    })
    .catch((err) => {
      console.log("Failed to call API login() ", err);
      notifyConfig.value.message = "Login Failed";
      notifyConfig.value.color = "red-darken-3";
      notify.value.show();
    })
    .finally(() => {
      loadingBtn.value = false;
    });
};

const onSubmit = (params) => {
  formRegister.value.showLoading();
  createUser(params)
    .then((response) => {
      notifyConfig.value.message = "Successfully to register a new user";
      notifyConfig.value.color = "success";

      formRegister.value.emptyForm();
      formRegister.value.hide();
    })
    .catch((err) => {
      console.log("Failed to call API createUser() ", err);
      notifyConfig.value.message = "Failed to register a new user";
      notifyConfig.value.color = "success";
    })
    .finally(() => {
      notify.value.show();
      formRegister.value.hideLoading();
    });
};

const showFormRegister = () => {
  formRegister.value.show();
};
</script>
