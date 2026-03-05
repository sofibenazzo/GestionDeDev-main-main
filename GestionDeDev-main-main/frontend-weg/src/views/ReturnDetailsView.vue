<script setup lang="ts">
import { ref, onMounted, reactive } from "vue";
import { useRoute, useRouter } from "vue-router";
import Sidebar from "../components/Sidebar.vue";
import api from "../services/api";
import { useAuth } from "../store/auth";
import {
  ChevronLeft,
  Printer,
  MessageSquare,
  History,
  User,
  Calendar,
  ClipboardCheck,
  Package,
  Wrench,
  Loader2,
  FileText,
  Paperclip
} from "lucide-vue-next";

const route = useRoute();
const router = useRouter();
const auth = useAuth();
const loading = ref(true);
const remito = ref<any>(null);
const personalList = ref<any[]>([]);

// Service Mode State
const serviceMode = reactive({
  active: false,
  saving: false,
  observacion: "",
  decision: "",
  personalId: null as number | null,
  nuevoEstadoId: null as number | null
});

const fetchRemito = async () => {
  try {
    const rawId = route.params.id;
    if (!rawId) return;
    const id = rawId.toString().replace("DEV-", "");
    const response = await api.get(`/api/Remito/${id}`);
    remito.value = response.data;
  } catch (err) {
    console.error("Error fetching remito details:", err);
  } finally {
    loading.value = false;
  }
};

const fetchMetadata = async () => {
  try {
    const resPersonal = await api.get("/api/Personal");
    personalList.value = resPersonal.data;
    if (personalList.value.length > 0) serviceMode.personalId = personalList.value[0].personalId;
  } catch (err) {
    console.error("Error fetching metadata:", err);
  }
};

onMounted(() => {
  fetchRemito();
  fetchMetadata();
});

const submitServiceAction = async () => {
  if (!serviceMode.personalId || !serviceMode.nuevoEstadoId) {
    window.alert("Debe seleccionar el personal y el nuevo estado.");
    return;
  }

  serviceMode.saving = true;
  try {
    const id = remito.value.remitoId;
    
    // 1. Create Observation if provided
    let obsId = remito.value.observacionId;
    if (serviceMode.observacion) {
      const obsRes = await api.post("/api/Observacion", {
        descripcion: serviceMode.observacion,
        personalId: serviceMode.personalId,
        fecha: new Date().toISOString()
      });
      obsId = obsRes.data.observacionId;
    }

    // 2. Create Decision if provided
    let decId = remito.value.decisionAdoptadaId;
    if (serviceMode.decision) {
      const decRes = await api.post("/api/DecisionAdoptada", {
        descripcion: serviceMode.decision,
        personalId: serviceMode.personalId,
        fecha: new Date().toISOString()
      });
      decId = decRes.data.decisionAdoptadaId;
    }

    // 3. Update Remito with new state and IDs
    await api.put(`/api/Remito/${id}`, {
      ...remito.value,
      tipoEstadoId: serviceMode.nuevoEstadoId,
      observacionId: obsId,
      decisionAdoptadaId: decId,
      usuarioId: auth.user.value?.usuarioId || remito.value.usuarioId
    });

    serviceMode.active = false;
    serviceMode.observacion = "";
    serviceMode.decision = "";
    await fetchRemito();
  } catch (err) {
    console.error("Error in service mode action:", err);
    window.alert("Error al procesar la acción técnica.");
  } finally {
    serviceMode.saving = false;
  }
};

const printReport = () => {
  window.print();
};
</script>

<template>
  <div class="flex min-h-screen bg-[#fcfcfd]">
    <Sidebar />

    <main v-if="remito" class="flex-1 p-6 lg:p-12 overflow-y-auto no-scrollbar">
      <header class="mb-10">
        <button
          @click="router.back()"
          class="mb-6 flex items-center gap-2 text-slate-400 hover:text-slate-900 transition-colors font-bold text-xs uppercase tracking-widest"
        >
          <ChevronLeft class="h-4 w-4" />
          Volver al listado
        </button>
        
        <div class="flex flex-col md:flex-row md:items-end justify-between gap-6">
          <div class="space-y-4">
            <div class="flex items-center gap-3">
              <span class="rounded-xl bg-[#005792] px-3 py-1.5 text-xs font-black text-white shadow-xl shadow-blue-900/10 tracking-widest">
                DEV-{{ remito.remitoId.toString().padStart(4, "0") }}
              </span>
              <span
                :class="[
                  'inline-flex items-center rounded-full px-3 py-1 text-[10px] font-black uppercase tracking-widest shadow-sm ring-1',
                  remito.tipoEstado?.estado === 'Aceptada'
                    ? 'bg-emerald-50 text-emerald-600 ring-emerald-200'
                    : remito.tipoEstado?.estado === 'Rechazada'
                    ? 'bg-rose-50 text-rose-600 ring-rose-200'
                    : 'bg-amber-50 text-amber-600 ring-amber-200',
                ]"
              >
                {{ remito.tipoEstado?.estado || "Pendiente" }}
              </span>
            </div>
            <h1 class="text-4xl lg:text-5xl font-extrabold tracking-tight text-slate-900">
              Detalle del Remito
            </h1>
            <div class="flex flex-wrap items-center gap-6 text-slate-400 font-bold text-[10px] uppercase tracking-[0.2em]">
              <div class="flex items-center gap-2">
                <Calendar class="h-4 w-4 text-slate-500" />
                <span>{{ new Date(remito.fecha).toLocaleDateString() }}</span>
              </div>
              <div class="flex items-center gap-2">
                <User class="h-4 w-4 text-slate-500" />
                <span>Encargado: {{ remito.usuario?.nombre || 'S/D' }}</span>
              </div>
            </div>
          </div>
          
          <div class="flex gap-3">
            <button
              v-if="auth.user.value?.role === 'Administrador' && !serviceMode.active"
              @click="serviceMode.active = true"
              class="flex items-center justify-center gap-3 rounded-2xl bg-[#005792] px-8 py-4 font-black text-xs uppercase tracking-widest text-white shadow-xl shadow-blue-900/20 hover:bg-blue-700 transition-all active:scale-95"
            >
              <Wrench class="h-5 w-5" />
              Modo Servicio
            </button>
            <button
              @click="printReport"
              class="flex items-center justify-center gap-3 rounded-2xl bg-white border border-slate-200 px-8 py-4 font-black text-xs uppercase tracking-widest text-slate-600 shadow-sm hover:bg-slate-50 transition-all active:scale-95"
            >
              <Printer class="h-5 w-5" />
              Imprimir
            </button>
          </div>
        </div>
      </header>

      <div class="grid grid-cols-1 gap-8 lg:grid-cols-12">
        <!-- Main Content Area -->
        <div class="lg:col-span-8 space-y-8">
          
          <!-- Technician Action Panel (Service Mode) -->
          <section v-if="serviceMode.active" class="premium-card p-10 border-2 border-[#005792]/20 bg-blue-50/10 animate-in slide-in-from-top-4 duration-500">
            <div class="flex items-center justify-between mb-10">
              <h2 class="flex items-center gap-4 text-xl font-black tracking-tight text-slate-900 uppercase">
                <div class="h-10 w-10 flex items-center justify-center rounded-xl bg-[#005792] text-white">
                  <Wrench class="h-5 w-5" />
                </div>
                Información Técnica
              </h2>
              <button @click="serviceMode.active = false" class="text-xs font-black uppercase text-slate-500 hover:text-slate-900">Cerrar Terminal</button>
            </div>

            <div class="grid grid-cols-1 md:grid-cols-2 gap-8 mb-8">
              <div class="space-y-3">
                <label class="text-[10px] font-black uppercase tracking-widest text-slate-600 ml-1">Personal Técnico Responsable</label>
                <select v-model="serviceMode.personalId" class="w-full rounded-2xl border border-slate-200 bg-white p-4 text-sm font-bold text-slate-900 outline-none focus:ring-4 focus:ring-blue-500/5 transition-all">
                  <option v-for="p in personalList" :key="p.personalId" :value="p.personalId">{{ p.nombre }} ({{ p.legajo }})</option>
                </select>
              </div>
              <div class="space-y-3">
                <label class="text-[10px] font-black uppercase tracking-widest text-slate-600 ml-1">Estado de la Resolución Técnica</label>
                <div class="grid grid-cols-2 gap-2">
                  <button @click="serviceMode.nuevoEstadoId = 3" :class="['px-4 py-3 rounded-xl text-[10px] font-black uppercase tracking-widest transition-all', serviceMode.nuevoEstadoId === 3 ? 'bg-emerald-600 text-white shadow-lg' : 'bg-white border border-slate-200 text-slate-900 hover:bg-slate-50']">ACEPTAR</button>
                  <button @click="serviceMode.nuevoEstadoId = 4" :class="['px-4 py-3 rounded-xl text-[10px] font-black uppercase tracking-widest transition-all', serviceMode.nuevoEstadoId === 4 ? 'bg-rose-600 text-white shadow-lg' : 'bg-white border border-slate-200 text-slate-900 hover:bg-slate-50']">RECHAZAR</button>
                  <button @click="serviceMode.nuevoEstadoId = 2" :class="['px-4 py-3 rounded-xl text-[10px] font-black uppercase tracking-widest transition-all', serviceMode.nuevoEstadoId === 2 ? 'bg-amber-500 text-white shadow-lg' : 'bg-white border border-slate-200 text-slate-900 hover:bg-slate-50']">EN REVISIÓN</button>
                </div>
              </div>
            </div>

            <div class="space-y-8">
              <div class="space-y-3">
                <label class="text-[10px] font-black uppercase tracking-widest text-slate-600 ml-1">Informe de Hallazgos Operativos</label>
                <textarea v-model="serviceMode.observacion" rows="3" placeholder="Describa el fallo técnico o estado de los componentes detectados..." class="w-full rounded-2xl border border-slate-200 bg-white p-6 text-sm font-bold text-slate-900 outline-none focus:ring-4 focus:ring-blue-500/5 transition-all resize-none"></textarea>
              </div>
              <div class="space-y-3">
                <label class="text-[10px] font-black uppercase tracking-widest text-slate-600 ml-1">Dictamen de Ingeniería Corporativa</label>
                <textarea v-model="serviceMode.decision" rows="2" placeholder="Acción correctiva realizada o recomendación técnica..." class="w-full rounded-2xl border border-slate-200 bg-white p-6 text-sm font-bold text-slate-900 outline-none focus:ring-4 focus:ring-blue-500/5 transition-all resize-none"></textarea>
              </div>
            </div>

            <div class="mt-10 pt-8 border-t border-slate-100 flex justify-end">
              <button
                @click="submitServiceAction"
                :disabled="serviceMode.saving"
                class="flex items-center gap-3 rounded-2xl bg-[#005792] px-10 py-4 font-black text-xs uppercase tracking-widest text-white shadow-xl shadow-blue-900/20 hover:bg-blue-700 transition-all disabled:opacity-50"
              >
                <Loader2 v-if="serviceMode.saving" class="h-5 w-5 animate-spin" />
                <span v-if="!serviceMode.saving">Consolidar Acta de Servicio</span>
                <span v-else>Procesando...</span>
              </button>
            </div>
          </section>

          <!-- Client & General Info -->
          <div class="premium-card p-8 grid grid-cols-1 md:grid-cols-2 gap-8 shadow-xl shadow-slate-200/50">
            <div class="space-y-5">
              <h3 class="text-[10px] font-black uppercase tracking-[0.2em] text-[#005792]">Cliente</h3>
              <div class="space-y-1">
                <p class="text-xl font-black text-slate-900">{{ remito.cliente?.razonSocial }}</p>
                <p class="text-xs font-bold text-slate-400 italic">Identificador Maestro: {{ remito.cliente?.codigoCliente }}</p>
              </div>
              <p class="text-xs font-bold text-slate-500 leading-relaxed bg-slate-50 p-4 rounded-2xl border border-slate-100">
                {{ remito.cliente?.direccion?.calle }} {{ remito.cliente?.direccion?.numero }}<br>
                {{ remito.cliente?.direccion?.ciudad }}
              </p>
            </div>
            <div class="space-y-5">
              <h3 class="text-[10px] font-black uppercase tracking-[0.2em] text-[#005792]">Control de Existencias</h3>
              <div class="grid grid-cols-2 gap-4">
                <div class="bg-white border border-slate-100 shadow-sm rounded-2xl p-5 text-center group hover:border-[#005792] transition-colors">
                  <p class="text-3xl font-black text-[#005792] group-hover:scale-110 transition-transform">{{ remito.productos?.length || 0 }}</p>
                  <p class="mt-1 text-[8px] font-black uppercase text-slate-400 tracking-tighter">Posiciones</p>
                </div>
                <div class="bg-white border border-slate-100 shadow-sm rounded-2xl p-5 text-center group hover:border-[#005792] transition-colors">
                  <p class="text-3xl font-black text-[#005792] group-hover:scale-110 transition-transform">{{ remito.productos?.reduce((acc: number, p: any) => acc + (p.cantidad || 0), 0) || 0 }}</p>
                  <p class="mt-1 text-[8px] font-black uppercase text-slate-400 tracking-tighter">Unid. Físicas</p>
                </div>
              </div>
            </div>
          </div>

          <!-- Items Table -->
          <div class="premium-card p-8 shadow-xl shadow-slate-200/50">
            <div class="flex items-center justify-between mb-8">
              <h2 class="flex items-center gap-3 text-xl font-black tracking-tight text-slate-900">
                <Package class="h-6 w-6 text-[#005792]" />
                Productos
              </h2>
            </div>
            
            <div class="overflow-x-auto">
              <table class="w-full text-left min-w-[600px]">
                <thead>
                  <tr class="text-[10px] font-black uppercase tracking-widest text-slate-400 border-b border-slate-100">
                    <th class="pb-6 px-4">Producto</th>
                    <th class="pb-6 px-4">Configuración</th>
                    <th class="pb-6 px-4 text-center">Unid.</th>
                    <th class="pb-6 px-4">Serial / ID</th>
                  </tr>
                </thead>
                <tbody class="divide-y divide-slate-50">
                  <tr v-for="(prod, index) in remito.productos" :key="index" class="group transition-all hover:bg-slate-50/50">
                    <td class="py-6 px-4">
                      <p class="text-sm font-black text-slate-900">{{ prod.item }}</p>
                      <p class="text-[10px] font-bold text-[#005792] uppercase tracking-widest">{{ prod.modelo }}</p>
                    </td>
                    <td class="py-6 px-4">
                      <div class="flex gap-1.5 flex-wrap">
                        <span v-if="prod.cv" class="rounded-lg bg-slate-900 px-2 py-1 text-[8px] font-black text-white uppercase tracking-tighter">{{ prod.cv }} CV</span>
                        <span v-if="prod.rpm" class="rounded-lg bg-slate-50 px-2 py-1 text-[8px] font-black text-slate-600 uppercase tracking-tighter border border-slate-100">{{ prod.rpm }} RPM</span>
                        <span v-if="prod.voltaje" class="rounded-lg bg-slate-50 px-2 py-1 text-[8px] font-black text-slate-600 uppercase tracking-tighter border border-slate-100">{{ prod.voltaje }}V</span>
                      </div>
                    </td>
                    <td class="py-6 px-4 text-center">
                      <span class="inline-flex h-8 w-8 items-center justify-center rounded-xl bg-slate-50 border border-slate-100 text-xs font-black text-slate-900 group-hover:bg-[#005792] group-hover:text-white transition-all">{{ prod.cantidad }}</span>
                    </td>
                    <td class="py-6 px-4">
                      <div class="flex items-center gap-2 font-mono text-xs font-bold text-slate-500 tracking-tighter">
                        {{ prod.numeroSerie || 'SN-NOT-PROVIDED' }}
                      </div>
                    </td>
                  </tr>
                </tbody>
              </table>
            </div>
          </div>

          <!-- Motivo, Observaciones del operador y Archivo -->
          <div class="grid grid-cols-1 md:grid-cols-3 gap-8">
            <div class="premium-card p-8 shadow-xl shadow-slate-200/50">
              <h3 class="text-[10px] font-black uppercase tracking-[0.2em] text-[#005792] mb-4">
                <FileText class="h-4 w-4 inline mr-2" />Motivo
              </h3>
              <p class="text-sm font-bold text-slate-700">
                {{ remito.motivo || 'No especificado' }}
              </p>
            </div>
            <div class="premium-card p-8 shadow-xl shadow-slate-200/50">
              <h3 class="text-[10px] font-black uppercase tracking-[0.2em] text-[#005792] mb-4">Observaciones del Operador</h3>
              <p class="text-sm font-bold text-slate-700 italic">
                {{ remito.observacionTexto || 'Sin observaciones' }}
              </p>
            </div>
            <div class="premium-card p-8 shadow-xl shadow-slate-200/50">
              <h3 class="text-[10px] font-black uppercase tracking-[0.2em] text-[#005792] mb-4">
                <Paperclip class="h-4 w-4 inline mr-2" />Archivo Adjunto
              </h3>
              <a
                v-if="remito.archivoRemito"
                :href="`http://localhost:5299/uploads/${remito.archivoRemito}`"
                target="_blank"
                class="inline-flex items-center gap-2 text-sm font-bold text-[#005792] hover:underline"
              >
                Ver Archivo
              </a>
              <p v-else class="text-sm font-bold text-slate-400">Sin archivo adjunto</p>
            </div>
          </div>

          <!-- Professional Observation & Decision Panels -->
          <div class="grid grid-cols-1 md:grid-cols-2 gap-8">
            <!-- Observation Card -->
            <div class="premium-card p-10 border-l-4 border-l-purple-500 shadow-xl shadow-slate-200/40">
              <h2 class="mb-8 flex items-center gap-3 text-[10px] font-black uppercase tracking-[0.3em] text-slate-400">
                <MessageSquare class="h-4 w-4 text-purple-500" />
                Observaciones
              </h2>
              <div v-if="remito.observacion" class="space-y-6">
                <p class="text-sm font-bold leading-relaxed text-slate-700 italic border-l-2 border-purple-100 pl-4 py-2">
                  "{{ remito.observacion.descripcion }}"
                </p>
                <div class="flex items-center gap-3 bg-slate-50 p-3 rounded-2xl border border-slate-100">
                  <div class="h-8 w-8 flex items-center justify-center rounded-lg bg-white shadow-sm font-black text-[10px] text-purple-600 border border-purple-50">TW</div>
                  <div>
                    <p class="text-[9px] font-black text-slate-900 uppercase">Perito Responsable</p>
                    <p class="text-[10px] font-bold text-slate-400">{{ remito.observacion.personal?.nombre || 'Técnico Especialista' }}</p>
                  </div>
                </div>
              </div>
              <div v-else class="py-8 text-center bg-slate-50/50 rounded-3xl border border-dashed border-slate-200">
                <p class="text-[10px] font-black uppercase tracking-widest text-slate-300">Pendiente de Informe Técnico</p>
              </div>
            </div>

            <!-- Decision Card -->
            <div class="premium-card p-10 border-l-4 border-l-[#005792] shadow-xl shadow-slate-200/40">
              <h2 class="mb-8 flex items-center gap-3 text-[10px] font-black uppercase tracking-[0.3em] text-slate-400">
                <ClipboardCheck class="h-4 w-4 text-[#005792]" />
                Decisión
              </h2>
              <div v-if="remito.decision" class="space-y-6">
                <div class="!bg-[#005792] rounded-2xl p-6 text-white shadow-lg shadow-blue-900/20">
                  <p class="text-[8px] font-black uppercase tracking-[0.2em] opacity-60 mb-2">Dictamen de Ingeniería</p>
                  <p class="text-xs font-bold leading-relaxed">{{ remito.decision.descripcion || 'Sin descripción detallada del protocolo.' }}</p>
                </div>
                <div class="flex items-center gap-3 text-[#005792]">
                  <Calendar class="h-4 w-4" />
                  <span class="text-[10px] font-black uppercase tracking-widest">Consolidado: {{ new Date(remito.decision.fecha).toLocaleDateString() }}</span>
                </div>
              </div>
              <div v-else class="py-8 text-center bg-slate-50/50 rounded-3xl border border-dashed border-slate-200">
                <p class="text-[10px] font-black uppercase tracking-widest text-slate-300">A la espera de resolución</p>
              </div>
            </div>
          </div>
        </div>

        <!-- Sidebar Timeline & Metadata -->
        <div class="lg:col-span-4 space-y-8">
          <section class="premium-card p-8 border border-slate-100 shadow-xl shadow-slate-200/30">
            <h3 class="mb-10 flex items-center gap-3 text-[10px] font-black uppercase tracking-[0.3em] text-slate-400">
              <History class="h-4 w-4 text-[#005792]" />
              Cronología de Garantía
            </h3>
            
            <div class="relative pl-6 space-y-12 before:absolute before:left-0 before:top-1.5 before:h-full before:w-[1px] before:bg-slate-100">
              <!-- Created Event -->
              <div class="relative">
                <div class="absolute -left-[27.5px] top-0 h-4 w-4 rounded-full bg-slate-900 border-4 border-white shadow-md"></div>
                <div>
                  <p class="text-[11px] font-black text-slate-900 uppercase tracking-widest">Apertura de Expediente</p>
                  <p class="text-[10px] font-bold text-slate-400 mt-1 italic">{{ new Date(remito.fecha).toLocaleDateString() }} - {{ new Date(remito.fecha).toLocaleTimeString() }}</p>
                  <p class="mt-2 text-[9px] font-bold text-slate-500 bg-slate-50 p-2 rounded-lg border border-slate-100">Iniciado por: {{ remito.usuario?.nombre || 'Terminal 01' }}</p>
                </div>
              </div>

              <!-- Updated Event (Dynamic) -->
              <div v-if="remito.tipoEstadoId !== 1" class="relative">
                <div :class="['absolute -left-[27.5px] top-0 h-4 w-4 rounded-full border-4 border-white shadow-md', remito.tipoEstadoId === 3 ? 'bg-emerald-500' : remito.tipoEstadoId === 4 ? 'bg-rose-500' : 'bg-amber-500']"></div>
                <div>
                  <p :class="['text-[11px] font-black uppercase tracking-widest', remito.tipoEstadoId === 3 ? 'text-emerald-600' : remito.tipoEstadoId === 4 ? 'text-rose-600' : 'text-amber-600']">
                    Estado: {{ remito.tipoEstado?.estado }}
                  </p>
                  <p class="text-[10px] font-bold text-slate-400 mt-1 italic">Sincronizado vía Gateway Técnico</p>
                </div>
              </div>

              <!-- Pending/Complete Placeholder -->
              <div class="relative">
                <div class="absolute -left-[27.5px] top-0 h-4 w-4 rounded-full bg-slate-100 border-4 border-white"></div>
                <p class="text-[11px] font-black text-slate-300 uppercase tracking-widest">Cierre de Proceso</p>
              </div>
            </div>
          </section>

          <section class="premium-card p-10 bg-slate-50 border border-slate-200 border-dashed">
            <h4 class="text-[9px] font-black uppercase tracking-[0.4em] text-slate-400 mb-6">Información Operativa</h4>
            <div class="space-y-4">
              <div class="flex items-center justify-between">
                <span class="text-[10px] font-bold text-slate-500">ID de Transacción:</span>
                <span class="text-[10px] font-mono font-bold text-slate-900">#{{ remito.remitoId * 1234 }}</span>
              </div>
              <div class="flex items-center justify-between">
                <span class="text-[10px] font-bold text-slate-500">Protocolo WEG:</span>
                <span class="text-[10px] font-mono font-bold text-slate-900">ARG-2024-{{ remito.remitoId }}</span>
              </div>
            </div>
          </section>
        </div>
      </div>
    </main>

    <!-- Loading State -->
    <main v-else class="flex-1 p-12 flex items-center justify-center">
      <div class="flex flex-col items-center gap-6">
        <Loader2 class="h-12 w-12 animate-spin text-[#005792]" />
        <p class="text-[10px] font-black uppercase tracking-[0.3em] text-slate-400 animate-pulse">Sincronizando con Servidor Central WEG...</p>
      </div>
    </main>
  </div>
</template>

<style scoped>
@media print {
  .no-scrollbar { overflow: visible !important; }
  aside, button, .Modo-Servicio { display: none !important; }
  main { padding: 0 !important; }
  .premium-card { border: 1px solid #eee !important; box-shadow: none !important; margin-bottom: 2rem !important; }
}
</style>
