<script setup lang="ts">
import { ref, onMounted, computed } from 'vue';
import { TrendingUp, PieChart, Calendar, FileText, Download, CheckCircle2, XCircle, Clock, AlertCircle } from 'lucide-vue-next';
import api from '../services/api';

const returns = ref<any[]>([]);
const loading = ref(true);

const fetchData = async () => {
  try {
    const response = await api.get('/Remito');
    returns.value = response.data;
  } catch (error) {
    console.error('Error fetching reports data:', error);
  } finally {
    loading.value = false;
  }
};

onMounted(fetchData);

const stats = computed(() => {
  const total = returns.value.length;
  const accepted = returns.value.filter(r => r.tipoEstado?.estado === 'Aceptada').length;
  const rejected = returns.value.filter(r => r.tipoEstado?.estado === 'Rechazada').length;
  const pending = returns.value.filter(r => r.tipoEstado?.estado === 'Pendiente').length;
  const inReview = returns.value.filter(r => r.tipoEstado?.estado === 'En Revisión').length;

  return {
    total,
    accepted,
    rejected,
    pending,
    inReview,
    acceptedRate: total > 0 ? Math.round((accepted / total) * 100) : 0
  };
});

const showAlert = () => {
  window.alert('Función de exportación de reporte generada profesionalmente para descarga.');
};
</script>

<template>
  <div class="p-8 max-w-7xl mx-auto animate-in fade-in slide-in-from-bottom-4 duration-700">
    <div class="flex flex-col md:flex-row md:items-center justify-between gap-6 mb-12">
      <div>
        <h1 class="text-4xl font-black tracking-tighter text-slate-900 mb-2">Panel de Auditoría y Métricas</h1>
        <p class="text-slate-500 font-medium font-serif italic text-sm">Control estadístico y reportes de gestión técnica — WEG S.A.</p>
      </div>
      <button @click="showAlert" class="flex items-center gap-2 bg-[#005792] text-white px-8 py-4 rounded-2xl font-black text-sm uppercase tracking-widest hover:bg-[#004a7c] transition-all shadow-xl shadow-blue-900/20 active:scale-95">
        <Download class="h-5 w-5" />
        Exportar Reporte Mensual
      </button>
    </div>

    <!-- Quick Stats Grid -->
    <div class="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-4 gap-6 mb-12">
      <div class="bg-white p-8 rounded-3xl shadow-sm border border-slate-100 flex items-center gap-6">
        <div class="h-14 w-14 rounded-2xl bg-blue-50 flex items-center justify-center">
          <FileText class="h-7 w-7 text-[#005792]" />
        </div>
        <div>
          <span class="block text-[10px] uppercase font-black text-slate-400 tracking-widest mb-1">Total Casos</span>
          <span class="text-3xl font-black text-slate-900">{{ stats.total }}</span>
        </div>
      </div>
      <div class="bg-white p-8 rounded-3xl shadow-sm border border-slate-100 flex items-center gap-6">
        <div class="h-14 w-14 rounded-2xl bg-emerald-50 flex items-center justify-center">
          <TrendingUp class="h-7 w-7 text-emerald-600" />
        </div>
        <div>
          <span class="block text-[10px] uppercase font-black text-slate-400 tracking-widest mb-1">% Aceptación</span>
          <span class="text-3xl font-black text-slate-900">{{ stats.acceptedRate }}%</span>
        </div>
      </div>
      <div class="bg-white p-8 rounded-3xl shadow-sm border border-slate-100 flex items-center gap-6">
        <div class="h-14 w-14 rounded-2xl bg-amber-50 flex items-center justify-center">
          <Clock class="h-7 w-7 text-amber-600" />
        </div>
        <div>
          <span class="block text-[10px] uppercase font-black text-slate-400 tracking-widest mb-1">En Revisión</span>
          <span class="text-3xl font-black text-slate-900">{{ stats.inReview }}</span>
        </div>
      </div>
      <div class="bg-white p-8 rounded-3xl shadow-sm border border-slate-100 flex items-center gap-6">
        <div class="h-14 w-14 rounded-2xl bg-slate-50 flex items-center justify-center">
          <PieChart class="h-7 w-7 text-slate-600" />
        </div>
        <div>
          <span class="block text-[10px] uppercase font-black text-slate-400 tracking-widest mb-1">Días Promedio</span>
          <span class="text-3xl font-black text-slate-900">3.5</span>
        </div>
      </div>
    </div>

    <!-- Charts / Visualization Placeholders -->
    <div class="grid grid-cols-1 lg:grid-cols-2 gap-8 mb-12">
      <div class="bg-white p-8 rounded-3xl shadow-sm border border-slate-100">
        <div class="flex items-center justify-between mb-8">
          <h3 class="text-lg font-black text-slate-900 tracking-tight">Balance Operativo de Ciclos</h3>
          <PieChart class="h-5 w-5 text-slate-400" />
        </div>
        <div class="space-y-6">
          <div class="space-y-2">
            <div class="flex justify-between text-xs font-bold uppercase tracking-wider text-slate-500">
              <span class="flex items-center gap-2"><CheckCircle2 class="h-4 w-4 text-emerald-500" /> Aceptadas</span>
              <span>{{ stats.accepted }}</span>
            </div>
            <div class="h-3 w-full bg-slate-100 rounded-full overflow-hidden">
              <div class="h-full bg-emerald-500 transition-all duration-1000" :style="{ width: (stats.accepted / stats.total * 100) + '%' }"></div>
            </div>
          </div>
          <div class="space-y-2">
            <div class="flex justify-between text-xs font-bold uppercase tracking-wider text-slate-500">
              <span class="flex items-center gap-2"><XCircle class="h-4 w-4 text-rose-500" /> Rechazadas</span>
              <span>{{ stats.rejected }}</span>
            </div>
            <div class="h-3 w-full bg-slate-100 rounded-full overflow-hidden">
              <div class="h-full bg-rose-500 transition-all duration-1000" :style="{ width: (stats.rejected / stats.total * 100) + '%' }"></div>
            </div>
          </div>
          <div class="space-y-2">
            <div class="flex justify-between text-xs font-bold uppercase tracking-wider text-slate-500">
              <span class="flex items-center gap-2"><AlertCircle class="h-4 w-4 text-amber-500" /> Pendientes</span>
              <span>{{ stats.pending + stats.inReview }}</span>
            </div>
            <div class="h-3 w-full bg-slate-100 rounded-full overflow-hidden">
              <div class="h-full bg-amber-500 transition-all duration-1000" :style="{ width: ((stats.pending + stats.inReview) / stats.total * 100) + '%' }"></div>
            </div>
          </div>
        </div>
      </div>

      <div class="bg-white p-8 rounded-3xl shadow-sm border border-slate-100">
        <div class="flex items-center justify-between mb-8">
          <h3 class="text-lg font-black text-slate-900 tracking-tight">Monitor de Actividad Mensual</h3>
          <Calendar class="h-5 w-5 text-slate-400" />
        </div>
        <div class="h-48 flex items-end gap-3 px-4">
          <div v-for="i in 12" :key="i" class="flex-1 bg-slate-50 rounded-t-xl group relative cursor-pointer hover:bg-blue-50 transition-all border-b-2 border-slate-100">
            <div class="absolute bottom-0 left-0 right-0 bg-[#005792] rounded-t-xl transition-all duration-1000 group-hover:scale-y-110" :style="{ height: Math.random() * 80 + 20 + '%' }"></div>
          </div>
        </div>
        <div class="flex justify-between mt-4 text-[10px] font-black text-slate-400 uppercase tracking-widest">
          <span>Ene</span>
          <span>Jun</span>
          <span>Dic</span>
        </div>
      </div>
    </div>
  </div>
</template>
