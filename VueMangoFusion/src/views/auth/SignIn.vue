<template>
  <div class="container mt-5">
    <div class="row justify-content-center">
      <div class="col-md-10">
        <div class="card shadow d-flex flex-row">
          <img
            src="@/assets/hero.jpg"
            class="card-img-left img-fluid"
            style="width: 50%; object-fit: cover"
          />
          <div class="card-body p-5" style="width: 50%">
            <h2 class="text-center mb-4">登录</h2>
            <form @submit.prevent="onSignInSubmit">
              <div class="mb-3">
                <label for="email" class="form-label">邮箱</label>
                <input type="email" v-model="formObj.email" class="form-control" id="email" />
              </div>

              <div class="mb-3">
                <label for="password" class="form-label">密码</label>
                <input
                  type="password"
                  v-model="formObj.password"
                  class="form-control"
                  id="password"
                />
              </div>

              <div class="alert alert-danger" v-if="errorList.length > 0">
                <span class="d-block" v-for="error in errorList" :key="error"> {{ error }} </span>
              </div>

              <button :disabled="isLoading" type="submit" class="btn btn-secondary w-100">
                <span v-if="isLoading" class="spinner-border spinner-border-sm me-2"></span>
                登录
              </button>
            </form>

            <div class="text-center mt-3">
              <router-link :to="{ name: APP_ROUTE_NAMES.SIGN_UP }"
                >没有账号？立即注册</router-link
              >
            </div>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup>
import { ROLES } from '@/constants/constants'
import { APP_ROUTE_NAMES } from '@/constants/routeNames'
import { reactive, ref } from 'vue'
import { useAuthStore } from '@/stores/authStore'
const authStore = useAuthStore()
const formObj = reactive({
  email: '',
  password: '',
})

const isLoading = ref(false)

const errorList = reactive([])

const onSignInSubmit = async () => {
  isLoading.value = true
  errorList.length = 0
  console.log(formObj)
  if (formObj.email === undefined || formObj.email.length === 0) {
    errorList.push('邮箱不能为空。')
  }

  if (formObj.password === undefined || formObj.password.length === 0) {
    errorList.push('密码不能为空。')
  }
  if (errorList.length > 0) {
    isLoading.value = false
    return
  }

  try {
    const response = await authStore.signIn(formObj)
    console.log(response)
    if (response.success) {
      console.log('success')
    } else {
      if (response.message !== undefined) {
        response.message.split('--').forEach((error) => {
          errorList.push(error)
        })
      }
    }
  } catch (err) {
    errorList.push(err)
  } finally {
    isLoading.value = false
  }
}
</script>
