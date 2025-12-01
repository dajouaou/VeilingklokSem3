import { defineConfig } from "vite";
import react from "@vitejs/plugin-react";

export default defineConfig({
    plugins: [react()],

    server: {
        port: 5173,
        strictPort: true,
        open: true,
        proxy: {
            "/api": {
                target: "http://localhost:5242",
                changeOrigin: true,
                secure: false,
            },
            "/hubs": {
                target: "http://localhost:5242",
                ws: true,
                changeOrigin: true,
                secure: false,
            }
        }
    }
});
