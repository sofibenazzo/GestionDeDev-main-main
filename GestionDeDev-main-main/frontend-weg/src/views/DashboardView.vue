<script setup lang="ts">
import { ref, computed, onMounted } from "vue";
import { useRouter } from "vue-router";
import Sidebar from "../components/Sidebar.vue";
import api from "../services/api";
import {
  LayoutDashboard,
  Package,
  Plus,
  ArrowUpRight,
  ArrowDownLeft,
  Search,
  ChevronRight,
  CheckCircle,
  XCircle,
  Clock,
  ShieldCheck
} from "lucide-vue-next";

const router = useRouter();
const searchQuery = ref("");
const loading = ref(true);

const stats = ref([
  {
    label: "Devoluciones Totales",
    value: "0",
    description: "Solicitudes registradas",
    icon: Package,
    color: "text-[#005792]",
    bgColor: "bg-blue-50"
  },
  {
    label: "En Revisión",
    value: "0",
    description: "Pendientes de ingeniería",
    icon: Clock,
    color: "text-amber-500",
    bgColor: "bg-amber-50"
  },
  {
    label: "Aceptadas",
    value: "0",
    description: "Garantía aprobada",
    icon: CheckCircle,
    color: "text-emerald-500",
    bgColor: "bg-emerald-50"
  },
  {
    label: "Rechazadas",
    value: "0",
    description: "Sin falla técnica",
    icon: XCircle,
    color: "text-rose-500",
    bgColor: "bg-rose-50"
  },
]);

const recentReturns = ref<any[]>([]);

onMounted(async () => {
  try {
    const response = await api.get("/api/Remito");
    const remitos = response.data;

    recentReturns.value = remitos.map((r: any) => ({
      id: `DEV-${r.remitoId.toString().padStart(4, "0")}`,
      remitoId: r.remitoId,
      client: r.cliente?.razonSocial || "Cliente Desconocido",
      products: r.productos?.length || 0,
      date: r.fecha
        ? new Date(r.fecha).toLocaleDateString()
        : new Date().toLocaleDateString(),
      status: r.tipoEstado?.estado || "En Revisión",
    }));

    // Update stats
    stats.value[0].value = remitos.length.toString();
    stats.value[1].value = remitos.filter((r: any) => 
      r.tipoEstado?.estado === "En Revisión" || r.tipoEstado?.estado === "Pendiente"
    ).length.toString();
    stats.value[2].value = remitos.filter((r: any) => r.tipoEstado?.estado === "Aceptada").length.toString();
    stats.value[3].value = remitos.filter((r: any) => r.tipoEstado?.estado === "Rechazada").length.toString();
  } catch (err) {
    console.error("Error fetching dashboard data:", err);
  } finally {
    loading.value = false;
  }
});

const filteredReturns = computed(() => {
  if (!searchQuery.value) return recentReturns.value;
  const query = searchQuery.value.toLowerCase();
  return recentReturns.value.filter(
    (ret: any) =>
      ret.id.toLowerCase().includes(query) ||
      ret.client.toLowerCase().includes(query)
  );
});
</script>

<template>
  <div class="flex min-h-screen bg-[#fcfcfd]">
    <Sidebar />

    <main class="flex-1 overflow-y-auto p-8 lg:p-12 no-scrollbar">
      <header class="mb-12 flex flex-col md:flex-row md:items-center justify-between gap-6">
        <div>
          <h1 class="text-5xl font-black tracking-tighter text-slate-900 mb-2">
            Dashboard
          </h1>
          <p class="text-slate-500 font-bold uppercase tracking-widest text-[10px]">
             Monitor Operativo de Garantías — WEG EQUIPAMIENTOS ELÉCTRICOS S.A.
          </p>
        </div>
        <router-link
          to="/returns/new"
          class="flex items-center gap-3 rounded-2xl bg-[#005792] px-8 py-4 font-black uppercase tracking-widest text-white shadow-2xl shadow-blue-900/20 transition-all hover:bg-blue-700 hover:scale-105 active:scale-95"
        >
          <Plus class="h-5 w-5" />
          Nueva Operación
        </router-link>
      </header>

      <!-- Stats Grid -->
      <div class="mb-12 grid grid-cols-1 gap-6 md:grid-cols-2 lg:grid-cols-4">
        <div v-for="stat in stats" :key="stat.label" class="premium-card p-8 group hover:shadow-2xl hover:shadow-slate-200/50 transition-all">
          <div class="mb-6 flex items-center justify-between">
            <div :class="['rounded-2xl p-4 transition-transform group-hover:scale-110', stat.bgColor, stat.color]">
              <component :is="stat.icon" class="h-8 w-8" />
            </div>
          </div>
          <p class="text-[10px] font-black uppercase tracking-[0.2em] text-slate-400 mb-1">
            {{ stat.label }}
          </p>
          <p class="text-4xl font-black text-slate-900 mb-2">
            {{ stat.value }}
          </p>
          <p class="text-[10px] font-bold text-slate-400 italic">{{ stat.description }}</p>
        </div>
      </div>

      <!-- Main Section -->
      <div class="grid grid-cols-1 lg:grid-cols-12 gap-8">
        <!-- Recent Activity Table -->
        <div class="lg:col-span-8">
          <div class="premium-card overflow-hidden h-full">
            <div class="flex items-center justify-between border-b border-slate-50 bg-white/50 p-8">
              <h2 class="text-2xl font-black tracking-tight text-slate-900">
                Resumen de Movimientos
              </h2>
              <div class="relative group">
                <Search class="absolute left-3 top-1/2 h-4 w-4 -translate-y-1/2 text-slate-400 group-focus-within:text-[#005792]" />
                <input
                  v-model="searchQuery"
                  type="text"
                  placeholder="Filtrar por ID o Cliente..."
                  class="w-48 rounded-xl border border-slate-100 bg-slate-50/50 py-2.5 pl-10 pr-4 text-xs font-bold text-slate-900 outline-none focus:border-[#005792] focus:bg-white focus:ring-4 focus:ring-blue-500/5 transition-all"
                />
              </div>
            </div>

            <div class="overflow-x-auto">
              <table class="w-full text-left">
                <thead>
                  <tr class="text-[10px] font-black uppercase tracking-widest text-slate-400">
                    <th class="py-6 px-8">N° Radicado</th>
                    <th class="py-6 px-4">Titular de Operación</th>
                    <th class="py-6 px-4">Estado Actual</th>
                    <th class="py-6 pr-8"></th>
                  </tr>
                </thead>
                <tbody class="divide-y divide-slate-50">
                  <tr
                    v-for="ret in filteredReturns.slice(0, 5)"
                    :key="ret.id"
                    class="group hover:bg-slate-50/80 transition-all cursor-pointer"
                    @click="router.push(`/returns/${ret.remitoId}`)"
                  >
                    <td class="py-6 px-8">
                      <span class="rounded-lg bg-slate-100 px-2 py-1 text-xs font-bold text-slate-600">{{ ret.id }}</span>
                    </td>
                    <td class="py-6 px-4">
                      <p class="text-sm font-black text-slate-900">{{ ret.client }}</p>
                      <p class="text-[9px] text-slate-400 uppercase font-black tracking-tighter">Ingreso: {{ ret.date }}</p>
                    </td>
                    <td class="py-6 px-4">
                      <span
                        :class="[
                          'inline-flex items-center rounded-full px-3 py-1 text-[9px] font-black uppercase tracking-widest shadow-sm ring-1',
                          ret.status === 'Aceptada'
                            ? 'bg-emerald-50 text-emerald-600 ring-emerald-200'
                            : ret.status === 'Rechazada'
                            ? 'bg-rose-50 text-rose-600 ring-rose-200'
                            : 'bg-amber-50 text-amber-600 ring-amber-200',
                        ]"
                      >
                        {{ ret.status }}
                      </span>
                    </td>
                    <td class="py-6 pr-8 text-right">
                      <ChevronRight class="h-5 w-5 text-slate-300 group-hover:text-[#005792] transition-colors" />
                    </td>
                  </tr>
                </tbody>
              </table>
            </div>
            <div class="p-6 border-t border-slate-50 bg-slate-50/20 text-center">
               <router-link to="/returns" class="text-xs font-black uppercase tracking-widest text-[#005792] hover:underline">Acceder al Histórico Completo</router-link>
            </div>
          </div>
        </div>

        <!-- Sidebar Info -->
        <div class="lg:col-span-4 space-y-8">
          <!-- Fixed Background and Contrast here -->
          <div class="premium-card p-10 !bg-[#005792] text-white overflow-hidden relative border-none">
            <div class="absolute -right-10 -bottom-10 h-40 w-40 rounded-full bg-white opacity-10"></div>
            <ShieldCheck class="h-12 w-12 mb-8 text-white/40" />
            <h3 class="text-2xl font-black mb-4 tracking-tight leading-tight">Garantía Certificada WEG</h3>
            <p class="text-sm text-blue-50 font-medium leading-relaxed mb-8">
              Todos los productos WEG cuentan con trazabilidad total. El sistema asegura una respuesta de ingeniería en menos de 72hs hábiles.
            </p>
            <div class="pt-8 border-t border-white/20 flex items-center gap-4">
               <div class="h-12 w-12 rounded-2xl bg-white/10 flex items-center justify-center">
                  <Package class="h-6 w-6 text-white" />
               </div>
               <div>
                  <p class="text-[10px] font-black uppercase tracking-widest text-white/50">Mantenimiento de Sistema</p>
                  <p class="text-xs font-bold text-white">Ingeniería Web S.A.</p>
               </div>
            </div>
          </div>
        </div>
      </div>
    </main>
  </div>
</template>
