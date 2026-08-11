import api from '@/services/api'

export default {
  /**
   * AI 自然语言搜索菜品
   * @param {string} query 用户自然语言输入，如"我想吃辣的便宜的菜"
   * @returns {{ keywords: string[], menuItems: array, originalQuery: string }}
   */
  async naturalLanguageSearch(query) {
    try {
      const response = await api.post('/Ai/search', { query })

      if (response.data.isSuccess) {
        return response.data.result
      } else {
        // 服务端返回了友好提示（如未能提取关键词）
        throw new Error(
          response.data.errorMessages?.[0] || 'AI 搜索失败，请稍后重试',
        )
      }
    } catch (error) {
      console.error('AI 自然语言搜索失败:', error)
      throw error
    }
  },

  /**
   * [管理端] AI 生成菜品营销描述文案
   * @param {{ menuItemId: number, name: string, price: number, category?: string, flavorStyle?: string }} data
   * @returns {{ description: string, model: string, totalTokens: number }}
   */
  async generateDescription(data) {
    try {
      const response = await api.post('/Ai/generate-description', data)

      if (response.data.isSuccess) {
        return response.data.result
      } else {
        throw new Error(
          response.data.errorMessages?.[0] || 'AI 生成文案失败，请稍后重试',
        )
      }
    } catch (error) {
      console.error('AI 生成营销文案失败:', error)
      throw error
    }
  },

  /**
   * [管理端] 将 AI 生成的描述应用到指定菜品
   * @param {{ menuItemId: number, description: string }} data
   */
  async applyDescription(data) {
    try {
      const response = await api.patch('/Ai/apply-description', data)

      if (response.data.isSuccess) {
        return response.data.result
      } else {
        throw new Error(
          response.data.errorMessages?.[0] || '应用描述失败，请稍后重试',
        )
      }
    } catch (error) {
      console.error('应用AI描述失败:', error)
      throw error
    }
  },
}
