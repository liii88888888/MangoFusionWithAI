<template>
  <div>
    <div class="position-relative overflow-hidden mb-4">
      <div class="hero-section position-relative py-5 rounded-4" style="min-height: 400px">
        <div class="container position-relative z-3">
          <div class="row justify-content-center text-center">
            <div class="col-lg-8 col-xl-7">
              <h1 class="display-4 fw-bold text-white mb-4">
                解锁美食艺术<br class="d-none d-lg-block" />
                <span class="text-success-emphasis">您的美食之旅从这里开始！</span>
              </h1>
              <div
                class="input-group mx-auto shadow-lg rounded-pill overflow-hidden"
                style="max-width: 600px"
              >
                <input
                  type="text"
                  v-model="searchValue"
                  class="form-control border-0 py-3 px-4"
                  placeholder="试试用自然语言搜索，如「想吃辣的」「清淡的适合小孩」..."
                  @keyup.enter="performAiSearch"
                />
                <button
                  class="btn btn-success px-4 d-flex align-items-center border-0"
                  @click="performAiSearch"
                  :disabled="isAiSearching"
                >
                  <span
                    v-if="isAiSearching"
                    class="spinner-border spinner-border-sm me-2"
                    role="status"
                  ></span>
                  <i v-else class="bi bi-search"></i>
                  <span class="ms-2 d-none d-sm-inline">{{ isAiSearching ? 'AI 思考中...' : 'AI 搜索' }}</span>
                </button>
              </div>
              <!-- AI 搜索结果提示 -->
              <div v-if="aiSearchMode" class="mt-3">
                <span class="badge bg-success-subtle text-success px-3 py-2 rounded-pill">
                  🤖 AI 智能搜索："{{ lastQuery }}"
                  <button
                    class="btn-close btn-close-sm ms-2"
                    style="font-size: 0.6rem"
                    @click="clearAiSearch"
                    title="清除AI搜索"
                  ></button>
                </span>
                <small class="text-body-secondary ms-2" v-if="aiKeywords.length">
                  识别关键词：{{ aiKeywords.join('、') }}
                </small>
              </div>
            </div>
          </div>
        </div>
      </div>
    </div>

    <div class="container px-0 mx-0">
      <!-- Filters Section -->
      <div class="row g-3 my-4 border align-items-center shadow-sm rounded-4 mx-1 pt-1 p-3">
        <!-- Categories -->
        <div class="col-lg-auto">
          <div class="d-flex flex-wrap gap-2">
            <button
              :class="{
                'btn-success shadow-sm': category === selectedCategory,
                'btn-outline-success': category !== selectedCategory,
              }"
              class="btn rounded px-4 py-2 fs-7 position-relative overflow-hidden"
              @click="updateSelectedCategory(category)"
              v-for="(category, index) in categoryList"
              :key="index"
            >
              <span class="position-relative z-1">{{ category }}</span>
            </button>
          </div>
        </div>

        <div class="col-lg-auto order-1 order-lg-2 ms-lg-auto">
          <div class="dropdown">
            <button
              class="btn btn-outline-success rounded px-3 py-2 dropdown-toggle d-flex align-items-center gap-2"
              type="button"
              data-bs-toggle="dropdown"
            >
              <i class="bi bi-sort-down"></i>
              <span class="fs-7">{{ selectedSortOption }}</span>
            </button>
            <ul class="dropdown-menu dropdown-menu-end shadow-sm rounded-3">
              <li v-for="(sort, index) in SORT_OPTIONS" :key="index">
                <button
                  class="dropdown-item py-2 px-3 d-flex align-items-center gap-2"
                  @click="updateSelectedSortOption(sort)"
                >
                  <span class="fs-7 px-1 mx-1">{{ sort }}</span>
                </button>
              </li>
            </ul>
          </div>
        </div>
      </div>

      <!-- Content Section -->
      <div class="text-center py-5" v-if="loading">
        <div class="spinner-border text-success" role="status">
          <span class="visually-hidden">Loading...</span>
        </div>
      </div>
      <div v-else>
        <div class="row">
          <MenuItemCard
            v-if="filteredItems.length && filteredItems.length > 0"
            v-for="(item, index) in filteredItems"
            :key="item.id"
            class="list-item col-12 col-md-6 col-lg-4 pb-4"
            :menuItem="item"
            @show-details="handleShowDetails"
          ></MenuItemCard>

          <div
            v-if="filteredItems.length === 0"
            class="text-center py-5 display-4 mx-auto text-body-secondary mb-3 d-block"
          >
            <i class="bi bi-emoji-frown"></i>
            <p class="lead text-body-secondary">没有找到符合条件的美食</p>
          </div>
        </div>
      </div>
    </div>

    <!-- Menu Detail Modal -->
    <MenuItemDetailsModal
      :show="showModal"
      :menuItem="selectedMenuItem"
      @close="handleCloseDetailsModal"
    ></MenuItemDetailsModal>
  </div>
</template>

<script setup>
import MenuItemDetailsModal from '@/components/modals/MenuItemDetailsModal.vue'
import MenuItemCard from '@/components/card/MenuItemCard.vue'
import menuItemService from '@/services/menuItemService.js'
import aiService from '@/services/aiService.js'
import { ref, onMounted, reactive, computed } from 'vue'
import { APP_ROUTE_NAMES } from '@/constants/routeNames'
import { CONFIG_IMAGE_URL } from '@/constants/config'
import { useSwal } from '@/composables/swal'
import { useRouter } from 'vue-router'
import {
  CATEGROIES,
  SORT_NAME_A_Z,
  SORT_NAME_Z_A,
  SORT_OPTIONS,
  SORT_PRICE_HIGH_LOW,
  SORT_PRICE_LOW_HIGH,
} from '@/constants/constants'
const { showConfirm, showError, showSuccess } = useSwal()
const menuItems = reactive([])
const loading = ref(false)
const selectedCategory = ref('全部')
const selectedSortOption = ref(SORT_OPTIONS[0])
const searchValue = ref('')

// ===== AI 自然语言搜索状态 =====
const isAiSearching = ref(false)
const aiSearchMode = ref(false)       // 是否处于 AI 搜索结果模式
const aiSearchResults = ref([])        // AI 返回的菜品列表
const aiKeywords = ref([])             // AI 提取的关键词
const lastQuery = ref('')              // 最后一次搜索的查询文本
const router = useRouter()
const categoryList = reactive(['全部', ...CATEGROIES])
const showModal = ref(false)
const selectedMenuItem = ref(null)

const handleShowDetails = (menuItem) => {
  selectedMenuItem.value = menuItem
  showModal.value = true
}

const handleCloseDetailsModal = (menuItem) => {
  selectedMenuItem.value = null
  showModal.value = false
}

function updateSelectedCategory(category) {
  selectedCategory.value = category
}
function updateSelectedSortOption(sort) {
  selectedSortOption.value = sort
}
const filteredItems = computed(() => {
  // ===== AI 搜索结果模式 =====
  if (aiSearchMode.value) {
    let tempArray = [...aiSearchResults.value]

    // 仍支持分类筛选
    if (selectedCategory.value !== '全部') {
      tempArray = tempArray.filter(
        (item) => item.category.toUpperCase() === selectedCategory.value.toUpperCase(),
      )
    }

    // 排序
    if (selectedSortOption.value == SORT_NAME_A_Z)
      tempArray.sort((a, b) => a.name.localeCompare(b.name))
    if (selectedSortOption.value == SORT_NAME_Z_A)
      tempArray.sort((a, b) => b.name.localeCompare(a.name))
    if (selectedSortOption.value == SORT_PRICE_LOW_HIGH)
      tempArray.sort((a, b) => a.price - b.price)
    if (selectedSortOption.value == SORT_PRICE_HIGH_LOW)
      tempArray.sort((a, b) => b.price - a.price)

    return tempArray
  }

  // ===== 本地过滤模式（原有逻辑）=====
  let tempArray =
    selectedCategory.value == '全部'
      ? [...menuItems]
      : menuItems.filter(
          (item) => item.category.toUpperCase() === selectedCategory.value.toUpperCase(),
        )

  if (searchValue.value) {
    tempArray = tempArray.filter((item) =>
      item.name.toUpperCase().includes(searchValue.value.toUpperCase()),
    )
  }

  if (selectedSortOption.value == SORT_NAME_A_Z) {
    tempArray.sort((a, b) => a.name.localeCompare(b.name))
  }
  if (selectedSortOption.value == SORT_NAME_Z_A) {
    tempArray.sort((a, b) => b.name.localeCompare(a.name))
  }
  if (selectedSortOption.value == SORT_PRICE_LOW_HIGH) {
    tempArray.sort((a, b) => a.price - b.price)
  }
  if (selectedSortOption.value == SORT_PRICE_HIGH_LOW) {
    tempArray.sort((a, b) => b.price - a.price)
  }

  return tempArray
})

// ===== AI 自然语言搜索 =====
const performAiSearch = async () => {
  const query = searchValue.value.trim()

  // 输入为空 → 退出 AI 模式，恢复全部菜品
  if (!query) {
    clearAiSearch()
    return
  }

  isAiSearching.value = true
  try {
    const result = await aiService.naturalLanguageSearch(query)

    aiKeywords.value = result.keywords || []
    aiSearchResults.value = result.menuItems || []
    lastQuery.value = query
    aiSearchMode.value = true
  } catch (error) {
    console.error('AI 搜索失败，回退到本地搜索:', error)
    // 失败时回退到本地搜索模式
    aiSearchMode.value = false
    aiKeywords.value = []
  } finally {
    isAiSearching.value = false
  }
}

const clearAiSearch = () => {
  aiSearchMode.value = false
  aiSearchResults.value = []
  aiKeywords.value = []
  lastQuery.value = ''
  searchValue.value = ''
}

const fetchMenuItems = async () => {
  menuItems.length = 0
  loading.value = true
  try {
    var result = await menuItemService.getMenuItems()
    menuItems.push(...result)
  } catch (error) {
    console.log('Error fetch menu items:', error)
  } finally {
    loading.value = false
  }
}

onMounted(fetchMenuItems)
</script>

<style scoped>
.hero-section {
  background:
    linear-gradient(rgba(0, 0, 0, 0.45), rgba(0, 0, 0, 0.45)), url('/src/assets/hero.jpg');
  background-size: cover;
  background-position: center;
  background-repeat: no-repeat;
}
.text-success-emphasis {
  color: #75e792 !important;
  font-weight: 400 !important;
}
</style>
