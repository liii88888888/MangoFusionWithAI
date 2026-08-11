<template>
  <div class="container py-5">
    <div class="card shadow-sm mb-4">
      <div class="card-header bg-success text-white">
        <h4 class="mb-0">订单摘要</h4>
      </div>
      <div class="card-body text-center">
        <div class="text-center my-2">
          <div class="alert alert-success d-inline-block px-5 py-3 fs-4" role="alert">
            <strong>谢谢！</strong> 您的订单已确认。
          </div>
        </div>
        <p><strong>订单号：</strong> #{{ orderId }}</p>
        <p><strong>日期：</strong>{{ new Date().toLocaleDateString() }}</p>
        <img
          src="/src/assets/confirm.jpg"
          alt="Order Confirmation"
          style="width: 500px; object-fit: cover"
          class="img-fluid rounded-3 mb-3"
        />
      </div>
      <p class="px-3 text-center">
        我们正在积极为您准备！请预留至少 30 分钟时间，确保每道菜品都以最新鲜的状态为您呈现。
      </p>
    </div>

    <div class="text-center">
      <a href="/" class="btn btn-success px-4">返回首页</a>
    </div>
  </div>
</template>

<script setup>
import { useRoute } from 'vue-router'
import { ref } from 'vue'
const route = useRoute()
const orderId = ref(route.params.orderId)
</script>
