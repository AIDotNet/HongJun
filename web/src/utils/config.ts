// 获取环境变量
export const config = {
    FAST_API_URL: import.meta.env.VITE_API_URL ?? "",
    NODE_ENV: import.meta.env.MODE,
    DEV: import.meta.env.DEV,
    GithuhClientID: import.meta.env.VITE_GITHUB_CLIENT_ID ?? "2d0aad1c7dd32416b8da",
  };
  