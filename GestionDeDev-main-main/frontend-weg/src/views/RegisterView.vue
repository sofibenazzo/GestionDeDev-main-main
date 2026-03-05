<script setup lang="ts">
import { ref, reactive } from "vue";
import { useRouter } from "vue-router";
import { Loader2 } from "lucide-vue-next";
import { useAuth } from "../store/auth";

import api from "../services/api";
import { jwtDecode } from "jwt-decode";

const router = useRouter();
const auth = useAuth();
const loading = ref(false);
const error = ref("");

const formData = reactive({
  nombreUsuario: "",
  email: "",
  password: "",
  rolId: 2, // Default to Analista de Garantías
});

const handleRegister = async () => {
  loading.value = true;
  error.value = "";
  try {
    const response = await api.post("/api/Auth/register", {
      nombreUsuario: formData.nombreUsuario,
      email: formData.email,
      password: formData.password,
      rolId: formData.rolId,
    });

    const token = response.data.token;
    const decoded: any = jwtDecode(token);

    auth.login(
      {
        usuarioId: parseInt(
          decoded[
            "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier"
          ]
        ),
        name:
          decoded[
            "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name"
          ] || "Usuario",
        role:
          decoded[
            "http://schemas.microsoft.com/ws/2008/06/identity/claims/role"
          ] === "1"
            ? "Administrador"
            : "Usuario",
        email:
          decoded[
            "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/emailaddress"
          ],
      },
      token
    );

    router.push("/");
  } catch (err: any) {
    error.value = err.response?.data?.message || "Error al registrar usuario.";
    console.error("Registration error:", err);
  } finally {
    loading.value = false;
  }
};
</script>

<template>
  <div
    class="flex flex-col min-h-screen items-center justify-center bg-[#fcfcfd] p-6 selection:bg-blue-600/10"
  >
    <!-- Sophisticated Background Pattern -->
    <div class="fixed inset-0 overflow-hidden -z-10">
      <div
        class="absolute top-[10%] left-[10%] h-[500px] w-[500px] rounded-full bg-blue-100/30 blur-[120px]"
      />
      <div
        class="absolute bottom-[10%] right-[10%] h-[500px] w-[500px] rounded-full bg-indigo-100/30 blur-[120px]"
      />
    </div>

    <div class="w-full max-w-md">
      <div
        class="premium-card bg-white p-8 md:p-12 shadow-2xl shadow-slate-200/50 animate-in fade-in zoom-in-95 duration-1000"
      >
        <div class="mb-10 text-center">
          <h2
            class="text-3xl font-black tracking-tighter text-slate-900 uppercase"
          >
            Solicitud de Registro
          </h2>
          <p class="mt-2 text-sm font-bold text-slate-400 uppercase tracking-widest">
            Alta de Nuevos Operarios de Planta
          </p>
        </div>

        <div
          v-if="error"
          class="mb-6 rounded-xl bg-rose-50 p-4 text-xs font-bold text-rose-600 ring-1 ring-rose-100 animate-in fade-in slide-in-from-top-2"
        >
          {{ error }}
        </div>

        <form @submit.prevent="handleRegister" class="space-y-6">
          <div class="space-y-2">
            <label
              class="text-[10px] font-black uppercase tracking-[0.2em] text-slate-400 pl-1"
              >Nombre y Apellido</label
            >
            <input
              v-model="formData.nombreUsuario"
              type="text"
              required
              placeholder="J.Perez"
              class="w-full rounded-2xl border border-slate-100 bg-slate-50 p-5 text-sm font-bold text-slate-900 placeholder-slate-300 outline-none transition-all focus:border-slate-300 focus:bg-white focus:ring-4 focus:ring-slate-900/5 shadow-sm"
            />
          </div>

          <div class="space-y-2">
            <label
              class="text-[10px] font-black uppercase tracking-[0.2em] text-slate-400 pl-1"
              >Email Corporativo</label
            >
            <input
              v-model="formData.email"
              type="email"
              required
              placeholder="juan.perez@weg.com"
              class="w-full rounded-2xl border border-slate-100 bg-slate-50 p-5 text-sm font-bold text-slate-900 placeholder-slate-300 outline-none transition-all focus:border-slate-300 focus:bg-white focus:ring-4 focus:ring-slate-900/5 shadow-sm"
            />
          </div>

          <div class="space-y-2">
            <label
              class="text-[10px] font-black uppercase tracking-[0.2em] text-slate-400 pl-1"
              >Contraseña</label
            >
            <input
              v-model="formData.password"
              type="password"
              required
              placeholder="••••••••"
              class="w-full rounded-2xl border border-slate-100 bg-slate-50 p-5 text-sm font-bold text-slate-900 placeholder-slate-300 outline-none transition-all focus:border-slate-300 focus:bg-white focus:ring-4 focus:ring-slate-900/5 shadow-sm"
            />
          </div>

          <div class="space-y-2">
            <label
              class="text-[10px] font-black uppercase tracking-[0.2em] text-slate-400 pl-1"
              >Rol Operativo</label
            >
            <select
              v-model="formData.rolId"
              class="w-full rounded-2xl border border-slate-100 bg-slate-50 p-5 text-sm font-bold text-slate-900 outline-none transition-all focus:border-slate-300 focus:bg-white focus:ring-4 focus:ring-slate-900/5 shadow-sm appearance-none"
            >
              <option :value="1">Administrador</option>
              <option :value="2">Usuario</option>
            </select>
          </div>

          <button
            type="submit"
            :disabled="loading"
            class="group relative flex w-full items-center justify-center rounded-2xl bg-slate-900 py-5 font-black uppercase tracking-widest text-white shadow-xl shadow-slate-900/20 transition-all hover:bg-black hover:scale-[1.02] active:scale-[0.98] disabled:opacity-50"
          >
            <Loader2 v-if="loading" class="mr-2 h-4 w-4 animate-spin" />
            <span v-else>Enviar Solicitud</span>
          </button>
        </form>

        <p
          class="mt-10 text-center text-[10px] font-black uppercase tracking-[0.2em] text-slate-400"
        >
          ¿Ya tienes cuenta?
          <router-link to="/login" class="text-slate-900 hover:underline"
            >Iniciar Sesión</router-link
          >
        </p>
      </div>
    </div>
  </div>
</template>
