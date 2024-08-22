import { inject } from 'vue';

export function useApp() {
  const showSnackbar = inject<(message: string) => void>('showSnackbar');
  
  if (!showSnackbar) {
    throw new Error('showSnackbar function not provided');
  }
  return {
    showSnackbar
  };
}
