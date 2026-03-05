<script setup lang="ts">
import { ref, onMounted } from "vue";
import { useRoute, useRouter } from "vue-router";
import Sidebar from "../components/Sidebar.vue";
import api from "../services/api";
import {
  ChevronLeft,
  Building2,
  MapPin,
  Package,
  Clock,
  Loader2,
  Mail,
  Phone,
  Globe,
  ExternalLink,
  ShieldCheck
} from "lucide-vue-next";
import { useAuth } from "../store/auth";

const route = useRoute();
const router = useRouter();
const auth = useAuth();
const client = ref<any>(null);
const loading = ref(true);

const showAlert = (m: string) => {
  if (typeof window !== 'undefined') window.alert(m);
};

onMounted(async () => {
  try {
    const response = await api.get(`/api/Cliente/${route.params.id}`);
    client.value = response.data;
  } catch (err) {
    console.error("Error fetching client details:", err);
  } finally {
    loading.value = false;
  }
});
</script>

<template>
  <div class="flex min-h-screen bg-[#fcfcfd]">
    <Sidebar />

    <main class="flex-1 p-6 lg:p-12 overflow-y-auto no-scrollbar">
      <div v-if="loading" class="flex h-[80vh] items-center justify-center">
        <div class="flex flex-col items-center gap-4">
          <Loader2 class="h-10 w-10 animate-spin text-[#005792]" />
          <p class="text-[10px] font-black uppercase tracking-widest text-slate-400">Consultando Base de Datos...</p>
        </div>
      </div>

      <div v-else-if="client" class="max-w-6xl mx-auto animate-in fade-in duration-700">
        <!-- Breadcrumb & Actions -->
        <div class="mb-10 flex flex-col md:flex-row md:items-center justify-between gap-6">
          <button
            @click="router.back()"
            class="flex items-center gap-2 text-xs font-black uppercase tracking-widest text-slate-400 hover:text-slate-900 transition-colors"
          >
            <ChevronLeft class="h-4 w-4" />
            Volver al Directorio
          </button>
          
          <div class="flex items-center gap-3">
            <button 
              v-if="auth.user.value?.role === 'Administrador'"
              @click="showAlert('Funcionalidad de edición en desarrollo')"
              class="rounded-xl border border-slate-200 bg-white px-5 py-3 text-[10px] font-black uppercase tracking-widest text-slate-600 hover:bg-slate-50 hover:text-slate-900 transition-all shadow-sm"
            >
              Editar Perfil
            </button>
            <button 
              @click="showAlert('Generando reporte PDF...')"
              class="rounded-xl bg-slate-900 px-5 py-3 text-[10px] font-black uppercase tracking-widest text-white shadow-xl shadow-slate-900/10 hover:bg-black transition-all"
            >
              Generar Reporte
            </button>
          </div>
        </div>

        <div class="grid grid-cols-1 lg:grid-cols-12 gap-8">
          <!-- Main Profile Cockpit -->
          <div class="lg:col-span-8 space-y-8">
            <section class="premium-card p-10 border border-slate-100 shadow-2xl shadow-slate-200/50 relative overflow-hidden">
              <!-- Decorative background element -->
              <div class="absolute -right-20 -top-20 h-64 w-64 rounded-full bg-blue-50/50"></div>
              
              <div class="relative flex flex-col md:flex-row items-start gap-8">
                <div class="flex h-24 w-24 shrink-0 items-center justify-center rounded-3xl bg-[#005792] text-white shadow-2xl shadow-blue-900/30">
                  <Building2 class="h-10 w-10" />
                </div>
                
                <div class="flex-1">
                  <div class="flex items-center gap-3 mb-2">
                    <span class="inline-flex items-center rounded-full bg-emerald-50 px-2.5 py-0.5 text-[10px] font-black uppercase tracking-widest text-emerald-600 ring-1 ring-emerald-200">
                      Certificación Activa
                    </span>
                    <span class="text-[10px] font-bold text-slate-500">ID Cliente #{{ client.clienteId }}</span>
                  </div>
                  <h1 class="text-4xl font-black text-slate-900 tracking-tight leading-tight">
                    {{ client.razonSocial }}
                  </h1>
                  <p class="mt-2 text-sm font-bold text-slate-600 uppercase tracking-[0.2em]">
                    Código Cliente: {{ client.codigoCliente }}
                  </p>
                </div>
              </div>
            </section>

            <!-- Location & Contact Grid -->
            <div class="grid grid-cols-1 md:grid-cols-2 gap-8">
              <section class="premium-card p-8 border border-slate-100 shadow-xl shadow-slate-200/40">
                <h3 class="mb-8 flex items-center gap-3 text-xs font-black uppercase tracking-widest text-[#005792]">
                  <MapPin class="h-4 w-4" />
                  Ubicación de Planta
                </h3>
                
                <div class="space-y-6">
                  <div>
                    <p class="text-[10px] font-black uppercase tracking-widest text-slate-500 mb-1">Calle y Número</p>
                    <p class="text-sm font-bold text-slate-900">
                      {{ client.direccion?.calle }} {{ client.direccion?.numero }}
                    </p>
                  </div>
                  <div>
                    <p class="text-[10px] font-black uppercase tracking-widest text-slate-500 mb-1">Ciudad</p>
                    <p class="text-sm font-bold text-slate-900">
                      {{ client.direccion?.ciudad || "No especificada" }}
                    </p>
                  </div>
                </div>
              </section>

              <section class="premium-card p-8 border border-slate-100 shadow-xl shadow-slate-200/40">
                <h3 class="mb-8 flex items-center gap-3 text-xs font-black uppercase tracking-widest text-[#005792]">
                  <Globe class="h-4 w-4" />
                  Contacto Corporativo
                </h3>
                
                <div class="space-y-4">
                  <div class="flex items-center gap-4 p-3 rounded-2xl bg-slate-50 border border-slate-200">
                    <Mail class="h-4 w-4 text-slate-500" />
                    <span class="text-xs font-bold text-slate-800">canal-oficial@{{ client.razonSocial.toLowerCase().replace(/\s/g, '') }}.com</span>
                  </div>
                  <div class="flex items-center gap-4 p-3 rounded-2xl bg-slate-50 border border-slate-200">
                    <Phone class="h-4 w-4 text-slate-500" />
                    <span class="text-xs font-bold text-slate-800">+54 (Consultar Registro)</span>
                  </div>
                </div>
              </section>
            </div>

            <!-- Recent Activity Placeholder -->
            <section class="premium-card p-8 border border-slate-100 bg-white shadow-lg shadow-slate-100/50">
               <div class="flex items-center justify-between mb-8">
                  <h3 class="flex items-center gap-3 text-xs font-black uppercase tracking-widest text-slate-400">
                    <Clock class="h-4 w-4" />
                    Monitor de Actividad Reciente
                  </h3>
                  <button class="text-[10px] font-black uppercase tracking-widest text-[#005792] hover:underline">Ver Historial</button>
               </div>
               
               <div class="space-y-4">
                 <div v-for="i in 2" :key="i" class="flex items-center justify-between p-4 rounded-2xl border border-slate-50 bg-slate-50/30 group hover:border-blue-100 hover:bg-white transition-all">
                    <div class="flex items-center gap-4">
                      <div class="h-10 w-10 flex items-center justify-center rounded-xl bg-white shadow-sm text-slate-400 group-hover:text-[#005792]">
                        <Package class="h-5 w-5" />
                      </div>
                      <div>
                        <p class="text-xs font-bold text-slate-900">Protocolo de Retorno DEV-00{{ i }}</p>
                        <p class="text-[10px] text-slate-400 font-medium tracking-tight">Cierre de Proceso hace 2 días</p>
                      </div>
                    </div>
                    <ExternalLink class="h-4 w-4 text-slate-300 group-hover:text-[#005792] transition-colors" />
                 </div>
               </div>
            </section>
          </div>

          <!-- Quick Actions & Stats -->
          <div class="lg:col-span-4 space-y-8">
            <section class="premium-card p-10 !bg-[#005792] text-white shadow-2xl shadow-blue-900/40 relative overflow-hidden border-none">
              <div class="absolute -right-8 -bottom-8 h-32 w-32 rounded-full bg-white/5 opacity-10"></div>
              <h3 class="mb-10 text-[10px] font-black uppercase tracking-widest text-white/50">Métricas de Falla</h3>
              
              <div class="space-y-8">
                <div>
                  <p class="text-5xl font-black mb-1">12</p>
                  <p class="text-[10px] font-black uppercase tracking-widest text-white/70">Incidencias Totales</p>
                </div>
                <div class="h-px bg-white/10 w-16"></div>
                <div>
                  <p class="text-5xl font-black mb-1">03</p>
                  <p class="text-[10px] font-black uppercase tracking-widest text-white/70">Garantías Activas</p>
                </div>
              </div>
            </section>

            <section class="premium-card p-8 border-l-4 border-l-emerald-500 bg-emerald-50/50">
              <div class="flex items-start gap-4">
                <ShieldCheck class="h-6 w-6 text-emerald-600 mt-1" />
                <div>
                  <h4 class="text-xs font-black uppercase tracking-widest text-emerald-900">Estado de Acreditación</h4>
                  <p class="mt-2 text-[11px] font-bold text-emerald-600 leading-relaxed shadow-sm">
                    Entidad comercial verificada y apta para procesos de post-venta técnica corporativa.
                  </p>
                </div>
              </div>
            </section>
          </div>
        </div>
      </div>

      <div v-else class="flex h-[80vh] items-center justify-center text-slate-400">
        <p class="font-bold">No se encontró el registro solicitado.</p>
      </div>
    </main>
  </div>
</template>
