<template>
  <div class="container mt-5">
    <div class="row justify-content-center">
      <div class="col-md-10">
        <div class="card shadow d-flex flex-row">
          <img
            src="@/assets/confirm.jpg"
            class="card-img-left img-fluid"
            style="width: 50%; object-fit: cover"
          />
          <div class="card-body p-5" style="width: 50%">
            <h2 class="text-center mb-4">注册</h2>

            <form @submit.prevent="onSignUpSubmit">
              <div class="mb-3">
                <label for="name" class="form-label">姓名</label>
                <input type="text" class="form-control" id="name" v-model="formObj.name" />
              </div>

              <div class="mb-3">
                <label for="email" class="form-label">邮箱</label>
                <input type="email" class="form-control" id="email" v-model="formObj.email" />
              </div>

              <div class="mb-3">
                <label for="role" class="form-label">角色</label>
                <select class="form-select" id="role" v-model="formObj.role">
                  <option v-for="role in ROLES" :key="role">{{ role }}</option>
                </select>
              </div>

              <div class="mb-3">
                <label for="password" class="form-label">密码</label>
                <input
                  type="password"
                  class="form-control"
                  v-model="formObj.password"
                  id="password"
                />
              </div>

              <div class="alert alert-danger" v-if="errorList.length > 0">
                <span class="d-block" v-for="error in errorList" :key="error"> {{ error }} </span>
              </div>

              <button :disabled="isLoading" type="submit" class="btn btn-secondary w-100">
                <span v-if="isLoading" class="spinner-border spinner-border-sm me-2"></span>
                注册
              </button>
            </form>

            <div class="text-center mt-3">
              <router-link :to="{ name: APP_ROUTE_NAMES.SIGN_IN }"
                >已有账号？立即登录</router-link
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
import { useAuthStore } from '@/stores/authStore'
import { reactive, ref } from 'vue'
const authStore = useAuthStore()
const formObj = reactive({
  name: '',
  email: '',
  password: '',
  role: 'Customer',
})

const isLoading = ref(false)

const errorList = reactive([])

const onSignUpSubmit = async () => {
  isLoading.value = true
  errorList.length = 0
  console.log(formObj)
  if (formObj.name === undefined || formObj.name.length === 0) {
    errorList.push('姓名不能为空。')
  }
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
    const response = await authStore.signUp(formObj)
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
