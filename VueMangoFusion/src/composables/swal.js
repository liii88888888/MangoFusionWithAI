import Swal from 'sweetalert2'

export function useSwal() {
  const showAlert = async (options) => {
    return await Swal.fire(options)
  }

  const showSuccess = async (message) => {
    return await showAlert({
      position: 'top',
      icon: 'success',
      title: message,
      showConfirmButton: false,
      timer: 1500,
    })
  }

  const showError = async (message) => {
    return await showAlert({
      position: 'top',
      icon: 'error',
      title: message,
      showConfirmButton: false,
      timer: 1500,
    })
  }

  const showConfirm = async (message, confirmText = '确定', cancelText = '取消') => {
    return await showAlert({
      title: '确定吗？',
      text: message,
      icon: 'warning',
      showCancelButton: true,
      confirmButtonColor: '#3085d6',
      cancelButtonColor: '#d33',
      confirmButtonText: confirmText,
      cancelButtonText: cancelText,
    })
  }

  return { showError, showSuccess, showConfirm }
}
