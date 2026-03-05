<script setup lang="ts">
import { ref, onMounted } from 'vue';
import { Plus, Search, Package, Edit, Trash2 } from 'lucide-vue-next';
import api from '../services/api';

const products = ref<any[]>([]);
const loading = ref(true);
const searchQuery = ref('');

const fetchProducts = async () => {
  try {
    const response = await api.get('/Producto');
    products.value = response.data;
  } catch (error) {
    console.error('Error fetching products:', error);
  } finally {
    loading.value = false;
  }
};

onMounted(fetchProducts);

const filteredProducts = () => {
  return products.value.filter(p => 
    p.item.toLowerCase().includes(searchQuery.value.toLowerCase()) ||
    p.modelo?.toLowerCase().includes(searchQuery.value.toLowerCase())
  );
};
</script>

<template>
  <div class="p-8 max-w-7xl mx-auto animate-in fade-in slide-in-from-bottom-4 duration-700">
    <div class="flex flex-col md:flex-row md:items-center justify-between gap-6 mb-12">
      <div>
        <h1 class="text-4xl font-black tracking-tighter text-slate-900 mb-2">Productos</h1>
        <p class="text-slate-500 font-medium">Control de trazabilidad y especificación de mercadería — WEG.</p>
      </div>
      <button class="flex items-center gap-2 bg-[#005792] text-white px-8 py-4 rounded-2xl font-black text-sm uppercase tracking-widest hover:bg-[#004a7c] transition-all shadow-xl shadow-blue-900/20 active:scale-95">
        <Plus class="h-5 w-5" />
        Nuevo Producto
      </button>
    </div>

    <!-- Search & Filters -->
    <div class="bg-white p-6 rounded-3xl shadow-sm border border-slate-100 mb-8">
      <div class="relative max-w-md">
        <Search class="absolute left-4 top-1/2 -translate-y-1/2 h-5 w-5 text-slate-400" />
        <input 
          v-model="searchQuery"
          type="text" 
          placeholder="Buscar producto por nombre o modelo..." 
          class="w-full pl-12 pr-6 py-4 bg-slate-50 border-none rounded-2xl text-sm font-medium focus:ring-2 focus:ring-[#005792] transition-all"
        />
      </div>
    </div>

    <!-- Products Table -->
    <div v-if="loading" class="flex flex-col items-center justify-center py-20">
      <div class="h-12 w-12 border-4 border-[#005792]/20 border-t-[#005792] rounded-full animate-spin"></div>
      <p class="mt-4 text-slate-500 font-bold uppercase tracking-widest text-xs">Cargando catálogo...</p>
    </div>

    <div v-else class="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-6">
      <div 
        v-for="product in filteredProducts()" 
        :key="product.productoId"
        class="bg-white p-6 rounded-3xl border border-slate-100 hover:shadow-2xl hover:shadow-slate-200/50 transition-all group"
      >
        <div class="flex items-start justify-between mb-6">
          <div class="h-14 w-14 rounded-2xl bg-blue-50 flex items-center justify-center group-hover:bg-[#005792] transition-colors">
            <Package class="h-7 w-7 text-[#005792] group-hover:text-white transition-colors" />
          </div>
          <div class="flex gap-2">
            <button class="p-2 text-slate-400 hover:text-[#005792] hover:bg-slate-50 rounded-xl transition-all">
              <Edit class="h-5 w-5" />
            </button>
            <button class="p-2 text-slate-400 hover:text-rose-600 hover:bg-rose-50 rounded-xl transition-all">
              <Trash2 class="h-5 w-5" />
            </button>
          </div>
        </div>

        <h3 class="text-xl font-black text-slate-900 mb-2 truncate">{{ product.item }}</h3>
        <p class="text-slate-500 font-medium text-sm mb-4">{{ product.modelo || 'Sin modelo' }}</p>

        <div class="grid grid-cols-2 gap-4 pt-4 border-t border-slate-50">
          <div>
            <span class="block text-[10px] uppercase font-black text-slate-400 tracking-widest mb-1">Potencia</span>
            <span class="text-sm font-bold text-slate-700">{{ product.cv || '-' }} CV</span>
          </div>
          <div>
            <span class="block text-[10px] uppercase font-black text-slate-400 tracking-widest mb-1">Voltaje</span>
            <span class="text-sm font-bold text-slate-700">{{ product.voltaje || '-' }} V</span>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>
