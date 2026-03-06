<script setup lang="ts">
import { ref, onMounted } from 'vue';
import { useRoute, useRouter } from 'vue-router';
import { ArrowLeft, Save, Loader2, Package } from 'lucide-vue-next';
import api from '../services/api';

const route = useRoute();
const router = useRouter();

const isEditing = ref(false);
const loading = ref(false);
const saving = ref(false);
const remitos = ref<any[]>([]);

const form = ref({
  productoId: 0,
  remitoId: null as number | null,
  item: '',
  cantidad: 1,
  cv: '',
  rpm: '',
  voltaje: '',
  protec: '',
  const: '',
  modelo: '',
  numeroSerie: ''
});

const fetchRemitos = async () => {
  try {
    const response = await api.get('/api/Remito');
    remitos.value = response.data;
  } catch (error) {
    console.error('Error fetching remitos:', error);
  }
};

const fetchProduct = async (id: string) => {
  loading.value = true;
  try {
    const response = await api.get(`/api/Producto/${id}`);
    const fp = response.data;
    form.value = {
      productoId: fp.productoId,
      remitoId: fp.remitoId,
      item: fp.item || '',
      cantidad: fp.cantidad || 1,
      cv: fp.cv || '',
      rpm: fp.rpm || '',
      voltaje: fp.voltaje || '',
      protec: fp.protec || '',
      const: fp.const || '',
      modelo: fp.modelo || '',
      numeroSerie: fp.numeroSerie || ''
    };
  } catch (error) {
    console.error('Error fetching product:', error);
    alert('Error al cargar el producto.');
    router.push('/products');
  } finally {
    loading.value = false;
  }
};

onMounted(async () => {
  await fetchRemitos();
  const id = route.params.id as string;
  if (id) {
    isEditing.value = true;
    await fetchProduct(id);
  }
});

const submitForm = async () => {
  if (!form.value.remitoId || !form.value.item || form.value.cantidad < 1) {
    alert('Por favor complete todos los campos obligatorios (Remito, Item, Cantidad).');
    return;
  }

  saving.value = true;
  try {
    const payload = { ...form.value };
    if (isEditing.value) {
      await api.put(`/api/Producto/${form.value.productoId}`, payload);
    } else {
      await api.post('/api/Producto', payload);
    }
    router.push('/products');
  } catch (error) {
    console.error('Error saving product:', error);
    alert('Error al guardar el producto.');
  } finally {
    saving.value = false;
  }
};
</script>

<template>
  <div class="flex-1 p-8 max-w-4xl mx-auto w-full animate-in fade-in slide-in-from-bottom-4 duration-700 bg-[#fcfcfd] min-h-screen">
    <!-- Header Actions -->
    <div class="mb-8">
      <button 
        @click="router.push('/products')"
        class="flex items-center gap-2 text-slate-500 hover:text-slate-900 font-medium transition-colors mb-6 group text-sm"
      >
        <div class="p-2 rounded-xl bg-white border border-slate-200 group-hover:border-slate-300 group-hover:shadow-sm transition-all">
          <ArrowLeft class="h-4 w-4" />
        </div>
        Volver a Fichas Técnicas
      </button>

      <div class="flex items-center gap-4">
        <div class="h-16 w-16 bg-blue-50 rounded-2xl flex items-center justify-center">
          <Package class="h-8 w-8 text-[#005792]" />
        </div>
        <div>
          <h1 class="text-3xl font-black text-slate-900 tracking-tight">
            {{ isEditing ? 'Editar Producto' : 'Nuevo Producto' }}
          </h1>
          <p class="text-slate-500 font-medium mt-1">
            {{ isEditing ? 'Actualiza los datos de la ficha técnica.' : 'Completa los datos para registrar un nuevo producto.' }}
          </p>
        </div>
      </div>
    </div>

    <div v-if="loading" class="flex flex-col items-center justify-center py-20 bg-white rounded-3xl shadow-sm border border-slate-100">
      <Loader2 class="h-12 w-12 animate-spin text-[#005792] mb-4" />
      <p class="text-sm font-bold text-slate-400 uppercase tracking-widest">Cargando datos...</p>
    </div>

    <!-- MAIN FORM -->
    <form v-else @submit.prevent="submitForm" class="bg-white rounded-3xl border border-slate-100 overflow-hidden shadow-sm">
      <div class="p-8">
        
        <h2 class="text-lg font-black text-slate-900 mb-6 flex items-center gap-2">
          <span class="w-8 h-8 rounded-lg bg-slate-100 text-slate-500 flex items-center justify-center text-xs">01</span>
          Información Principal
        </h2>

        <div class="grid grid-cols-1 md:grid-cols-2 gap-6">
          <div class="col-span-1 md:col-span-2 space-y-2">
            <label class="block text-xs font-black text-slate-400 uppercase tracking-widest">Remito Asociado <span class="text-rose-500">*</span></label>
            <select
              v-model="form.remitoId"
              class="w-full px-5 py-4 bg-slate-50 border-none rounded-2xl text-sm font-medium focus:ring-2 focus:ring-[#005792] transition-shadow"
              required
            >
              <option :value="null" disabled>Seleccione un remito</option>
              <option v-for="r in remitos" :key="r.remitoId" :value="r.remitoId">
                Remito #{{ r.numeroRemito }} - {{ r.cliente?.nombreFantasia || 'Sin cliente' }}
              </option>
            </select>
          </div>

          <div class="space-y-2">
            <label class="block text-xs font-black text-slate-400 uppercase tracking-widest">Item / Nombre <span class="text-rose-500">*</span></label>
            <input
              v-model="form.item"
              type="text"
              placeholder="Ej: Motor Trifásico"
              class="w-full px-5 py-4 bg-slate-50 border-none rounded-2xl text-sm font-medium focus:ring-2 focus:ring-[#005792] transition-shadow"
              required
            />
          </div>

          <div class="space-y-2">
            <label class="block text-xs font-black text-slate-400 uppercase tracking-widest">Cantidad <span class="text-rose-500">*</span></label>
            <input
              v-model="form.cantidad"
              type="number"
              min="1"
              class="w-full px-5 py-4 bg-slate-50 border-none rounded-2xl text-sm font-medium focus:ring-2 focus:ring-[#005792] transition-shadow"
              required
            />
          </div>
          
          <div class="space-y-2">
            <label class="block text-xs font-black text-slate-400 uppercase tracking-widest">Modelo</label>
            <input
              v-model="form.modelo"
              type="text"
              placeholder="Ej: W22"
              class="w-full px-5 py-4 bg-slate-50 border-none rounded-2xl text-sm font-medium focus:ring-2 focus:ring-[#005792] transition-shadow"
            />
          </div>

          <div class="space-y-2">
            <label class="block text-xs font-black text-slate-400 uppercase tracking-widest">Número de Serie</label>
            <input
              v-model="form.numeroSerie"
              type="text"
              placeholder="Ej: 10293847"
              class="w-full px-5 py-4 bg-slate-50 border-none rounded-2xl text-sm font-medium focus:ring-2 focus:ring-[#005792] transition-shadow"
            />
          </div>
        </div>

        <div class="w-full h-px bg-slate-100 my-8"></div>

        <h2 class="text-lg font-black text-slate-900 mb-6 flex items-center gap-2">
          <span class="w-8 h-8 rounded-lg bg-slate-100 text-slate-500 flex items-center justify-center text-xs">02</span>
          Especificaciones Técnicas
        </h2>

        <div class="grid grid-cols-1 md:grid-cols-3 gap-6">
          <div class="space-y-2">
            <label class="block text-xs font-black text-slate-400 uppercase tracking-widest">Potencia (CV)</label>
            <input
              v-model="form.cv"
              type="text"
              placeholder="Ej: 10 CV"
              class="w-full px-5 py-4 bg-slate-50 border-none rounded-2xl text-sm font-medium focus:ring-2 focus:ring-[#005792] transition-shadow"
            />
          </div>

          <div class="space-y-2">
            <label class="block text-xs font-black text-slate-400 uppercase tracking-widest">Voltaje / Tensión</label>
            <input
              v-model="form.voltaje"
              type="text"
              placeholder="Ej: 380V"
              class="w-full px-5 py-4 bg-slate-50 border-none rounded-2xl text-sm font-medium focus:ring-2 focus:ring-[#005792] transition-shadow"
            />
          </div>

          <div class="space-y-2">
            <label class="block text-xs font-black text-slate-400 uppercase tracking-widest">RPM</label>
            <input
              v-model="form.rpm"
              type="text"
              placeholder="Ej: 1500"
              class="w-full px-5 py-4 bg-slate-50 border-none rounded-2xl text-sm font-medium focus:ring-2 focus:ring-[#005792] transition-shadow"
            />
          </div>

          <div class="space-y-2">
            <label class="block text-xs font-black text-slate-400 uppercase tracking-widest">Protec.</label>
            <input
              v-model="form.protec"
              type="text"
              placeholder="Ej: IP55"
              class="w-full px-5 py-4 bg-slate-50 border-none rounded-2xl text-sm font-medium focus:ring-2 focus:ring-[#005792] transition-shadow"
            />
          </div>

          <div class="space-y-2 col-span-1 md:col-span-2">
            <label class="block text-xs font-black text-slate-400 uppercase tracking-widest">Forma Const.</label>
            <input
              v-model="form.const"
              type="text"
              placeholder="Ej: B3"
              class="w-full px-5 py-4 bg-slate-50 border-none rounded-2xl text-sm font-medium focus:ring-2 focus:ring-[#005792] transition-shadow"
            />
          </div>
        </div>

      </div>

      <!-- Footer Actions -->
      <div class="p-6 bg-slate-50 border-t border-slate-100 flex justify-end gap-3 rounded-b-[22px]">
        <button
          type="button"
          @click="router.push('/products')"
          class="px-6 py-3 text-sm font-bold text-slate-500 hover:text-slate-900 hover:bg-slate-200 rounded-xl transition-all"
        >
          Cancelar
        </button>
        <button
          type="submit"
          :disabled="saving"
          class="flex items-center gap-2 px-8 py-3 bg-[#005792] text-white rounded-xl text-sm font-bold shadow-lg shadow-blue-900/20 hover:bg-[#004a7c] transition-all disabled:opacity-50 disabled:cursor-not-allowed active:scale-95"
        >
          <Loader2 v-if="saving" class="h-4 w-4 animate-spin" />
          <Save v-else class="h-4 w-4" />
          {{ saving ? 'Guardando...' : (isEditing ? 'Guardar Cambios' : 'Crear Producto') }}
        </button>
      </div>
    </form>
  </div>
</template>
