import { defineConfig } from "vite";
import react from "@vitejs/plugin-react";
import basicSsl from "@vitejs/plugin-basic-ssl";

export default defineConfig({
  plugins: [react(), basicSsl()],
  server: {
    host: "localhost",
    port: 3000,
    strictPort: true,
    https: true,
    open: "/",
    proxy: {
      "/api": {
        target: "https://localhost:7183",
        changeOrigin: true,
        secure: false
      }
    }
  }
});
