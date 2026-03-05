<script setup lang="ts">
import { ref, computed, onMounted } from "vue";
import Sidebar from "../components/Sidebar.vue";
import api from "../services/api";
import { Search, Plus, ChevronRight, Filter } from "lucide-vue-next";
import { useAuth } from "../store/auth";

const searchQuery = ref("");
const loading = ref(true);
const returns = ref<any[]>([]);
const auth = useAuth();

onMounted(async () => {
  try {
    const response = await api.get("/api/Remito");
    returns.value = response.data.map((r: any) => ({
      id: r.remitoId ? `DEV-${r.remitoId.toString().padStart(4, "0")}` : 'DEV-????',
      remitoId: r.remitoId,
      client: r.cliente?.razonSocial || "Cliente Desconocido",
      products: r.productos?.length || 0,
      date: r.fecha
        ? new Date(r.fecha).toLocaleDateString()
        : new Date().toLocaleDateString(),
      rawDate: r.fecha,
      status: r.tipoEstado?.estado || "Pendiente",
      motivo: r.motivo || "",
    }));
  } catch (err) {
    console.error("Error fetching returns:", err);
  } finally {
    loading.value = false;
  }
});

const statusFilter = ref("");
const clientFilter = ref("");
const fechaDesde = ref("");
const fechaHasta = ref("");

// Unique clients for filter
const uniqueClients = computed(() => {
  const clients = returns.value.map(r => r.client);
  return [...new Set(clients)].sort();
});

const filteredReturns = computed(() => {
  let filtered = returns.value;

  if (statusFilter.value) {
    filtered = filtered.filter((ret) => ret.status === statusFilter.value);
  }

  if (clientFilter.value) {
    filtered = filtered.filter((ret) => ret.client === clientFilter.value);
  }

  if (fechaDesde.value) {
    const desde = new Date(fechaDesde.value);
    filtered = filtered.filter((ret) => new Date(ret.rawDate) >= desde);
  }

  if (fechaHasta.value) {
    const hasta = new Date(fechaHasta.value);
    hasta.setHours(23, 59, 59);
    filtered = filtered.filter((ret) => new Date(ret.rawDate) <= hasta);
  }

  if (searchQuery.value) {
    const query = searchQuery.value.toLowerCase();
    filtered = filtered.filter(
      (ret: any) =>
        ret.id.toLowerCase().includes(query) ||
        ret.client.toLowerCase().includes(query) ||
        ret.motivo.toLowerCase().includes(query)
    );
  }

  return filtered;
});

const showFilters = ref(false);

const clearFilters = () => {
  statusFilter.value = "";
  clientFilter.value = "";
  fechaDesde.value = "";
  fechaHasta.value = "";
  searchQuery.value = "";
};
</script>

<template>
  <div class="flex min-h-screen bg-[#fcfcfd]">
    <Sidebar />

    <main class="flex-1 p-6 lg:p-12 overflow-y-auto no-scrollbar">
      <header class="mb-10 flex flex-col md:flex-row md:items-center justify-between gap-6">
        <div>
          <h1 class="text-4xl font-black tracking-tight text-slate-900 mb-2">
            Devoluciones
          </h1>
          <p class="text-slate-400 font-bold text-xs uppercase tracking-widest">
            Gestión de Remitos de Mercadería — WEG S.A.
          </p>
        </div>
        <router-link
          to="/returns/new"
          class="flex items-center gap-3 rounded-2xl bg-[#005792] px-8 py-4 font-black uppercase tracking-widest text-white shadow-xl shadow-blue-900/20 transition-all hover:bg-blue-700 hover:scale-105 active:scale-95"
        >
          <Plus class="h-5 w-5" />
          Nuevo Remito
        </router-link>
      </header>

      <div class="premium-card overflow-hidden">
        <!-- Search & Filters Bar -->
        <div class="border-b border-slate-100 bg-white/50 p-6 space-y-4">
          <div class="flex flex-col md:flex-row items-center gap-4">
            <div class="relative group flex-1 w-full">
              <Search
                class="absolute left-4 top-1/2 h-4 w-4 -translate-y-1/2 text-slate-400 group-focus-within:text-[#005792] transition-colors"
              />
              <input
                v-model="searchQuery"
                type="text"
                placeholder="Buscar por ID, Cliente o Motivo..."
                class="w-full rounded-2xl border border-slate-100 bg-slate-50/50 py-4 pl-12 pr-6 text-sm font-bold text-slate-900 outline-none focus:border-[#005792] focus:bg-white focus:ring-4 focus:ring-blue-500/5 transition-all"
              />
            </div>

            <select
              v-model="statusFilter"
              class="rounded-2xl border border-slate-100 bg-white px-5 py-4 text-slate-600 hover:bg-slate-50 transition-all font-bold text-xs uppercase tracking-widest outline-none focus:ring-4 focus:ring-blue-500/5"
            >
              <option value="">Todos los Estados</option>
              <option value="Pendiente">Pendiente</option>
              <option value="En Revisión">En Revisión</option>
              <option value="Aceptada">Aceptada</option>
              <option value="Rechazada">Rechazada</option>
            </select>

            <button
              @click="showFilters = !showFilters"
              :class="['flex items-center gap-2 rounded-2xl border px-5 py-4 font-bold text-xs uppercase tracking-widest transition-all', showFilters ? 'border-[#005792] bg-blue-50 text-[#005792]' : 'border-slate-100 bg-white text-slate-500 hover:bg-slate-50']"
            >
              <Filter class="h-4 w-4" />
              Filtros
            </button>
          </div>

          <!-- Advanced Filters -->
          <div v-if="showFilters" class="grid grid-cols-1 md:grid-cols-3 gap-4 pt-4 border-t border-slate-100 animate-in fade-in slide-in-from-top-2 duration-300">
            <div class="space-y-2">
              <label class="text-[10px] font-black uppercase tracking-widest text-slate-400 ml-1">Cliente</label>
              <select
                v-model="clientFilter"
                class="w-full rounded-xl border border-slate-100 bg-white px-4 py-3 text-sm font-bold text-slate-900 outline-none focus:ring-4 focus:ring-blue-500/5"
              >
                <option value="">Todos los Clientes</option>
                <option v-for="c in uniqueClients" :key="c" :value="c">{{ c }}</option>
              </select>
            </div>
            <div class="space-y-2">
              <label class="text-[10px] font-black uppercase tracking-widest text-slate-400 ml-1">Fecha Desde</label>
              <input
                v-model="fechaDesde"
                type="date"
                class="w-full rounded-xl border border-slate-100 bg-white px-4 py-3 text-sm font-bold text-slate-900 outline-none focus:ring-4 focus:ring-blue-500/5"
              />
            </div>
            <div class="space-y-2">
              <label class="text-[10px] font-black uppercase tracking-widest text-slate-400 ml-1">Fecha Hasta</label>
              <input
                v-model="fechaHasta"
                type="date"
                class="w-full rounded-xl border border-slate-100 bg-white px-4 py-3 text-sm font-bold text-slate-900 outline-none focus:ring-4 focus:ring-blue-500/5"
              />
            </div>
            <div class="md:col-span-3 flex justify-end">
              <button @click="clearFilters" class="text-xs font-bold text-[#005792] uppercase tracking-widest hover:underline">
                Limpiar Filtros
              </button>
            </div>
          </div>

          <div class="flex items-center justify-between pt-2">
            <p class="text-[10px] font-black uppercase tracking-widest text-slate-300">
              {{ filteredReturns.length }} registros encontrados
            </p>
          </div>
        </div>

        <!-- Table -->
        <div class="overflow-x-auto">
          <table class="w-full text-left">
            <thead>
              <tr class="text-[10px] font-black uppercase tracking-widest text-slate-400 border-b border-slate-50">
                <th class="py-6 px-8">ID Remito</th>
                <th class="py-6 px-4">Cliente</th>
                <th class="py-6 px-4">Motivo</th>
                <th class="py-6 px-4">Items</th>
                <th class="py-6 px-4">Fecha</th>
                <th class="py-6 px-4">Estado</th>
                <th class="py-6 pr-8"></th>
              </tr>
            </thead>
            <tbody class="divide-y divide-slate-50">
              <tr
                v-for="ret in filteredReturns"
                :key="ret.id"
                class="group hover:bg-slate-50/80 transition-all cursor-pointer"
                @click="$router.push(`/returns/${ret.remitoId}`)"
              >
                <td class="py-6 px-8">
                  <span
                    class="rounded-lg bg-slate-100 px-2 py-1 text-xs font-bold text-slate-600"
                    >{{ ret.id }}</span
                  >
                </td>
                <td class="py-6 px-4">
                  <p class="text-sm font-bold text-slate-900">
                    {{ ret.client }}
                  </p>
                </td>
                <td class="py-6 px-4">
                  <p class="text-xs font-medium text-slate-500">
                    {{ ret.motivo || "—" }}
                  </p>
                </td>
                <td class="py-6 px-4 text-center">
                  <span class="text-sm font-semibold text-slate-600">{{
                    ret.products
                  }}</span>
                </td>
                <td class="py-6 px-4 text-sm font-medium text-slate-500">
                  {{ ret.date }}
                </td>
                <td class="py-6 px-4">
                  <span
                    :class="[
                      'inline-flex items-center rounded-full px-3 py-1 text-[10px] font-black uppercase tracking-widest shadow-sm ring-1',
                      ret.status === 'Aceptada'
                        ? 'bg-emerald-50 text-emerald-600 ring-emerald-200'
                        : ret.status === 'Rechazada'
                        ? 'bg-rose-50 text-rose-600 ring-rose-200'
                        : ret.status === 'En Revisión'
                        ? 'bg-amber-50 text-amber-600 ring-amber-200'
                        : 'bg-blue-50 text-blue-600 ring-blue-200',
                    ]"
                  >
                    {{ ret.status }}
                  </span>
                </td>
                <td class="py-6 pr-8 text-right">
                  <div
                    class="inline-flex h-10 w-10 items-center justify-center rounded-xl bg-slate-900 text-white shadow-lg shadow-slate-900/20 transition-all group-hover:scale-110 group-hover:bg-[#005792] active:scale-90"
                  >
                    <ChevronRight class="h-5 w-5" />
                  </div>
                </td>
              </tr>
              <tr v-if="filteredReturns.length === 0 && !loading">
                <td colspan="7" class="py-20 text-center">
                  <p class="text-sm font-bold text-slate-400">No se encontraron devoluciones con los filtros seleccionados.</p>
                </td>
              </tr>
            </tbody>
          </table>
        </div>
      </div>
    </main>
  </div>
</template>
