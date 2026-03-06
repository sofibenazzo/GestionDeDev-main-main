<script setup lang="ts">
import { ref, computed, onMounted } from 'vue';
import { ShieldUser, UserPlus, Search, Edit, Trash2, ShieldCheck, UserCircle, Loader2 } from 'lucide-vue-next';
import Sidebar from '../components/Sidebar.vue'; // ✅ CORREGIDO: faltaba Sidebar
import api from '../services/api';

const users = ref<any[]>([]);
const roles = ref<any[]>([]);
const loading = ref(true);
const searchQuery = ref('');

const fetchData = async () => {
  try {
    const [usersRes, rolesRes] = await Promise.all([
      api.get('/api/Usuario'), // ✅ CORREGIDO: era '/Usuario'
      api.get('/api/Rol')      // ✅ CORREGIDO: era '/Rol'
    ]);
    users.value = usersRes.data;
    roles.value = rolesRes.data;
  } catch (error) {
    console.error('Error fetching users/roles:', error);
  } finally {
    loading.value = false;
  }
};

onMounted(fetchData);

const getRoleName = (roleId: number) => {
  return roles.value.find(r => r.rolId === roleId)?.nombre || 'Usuario';
};

// ✅ CORREGIDO: era función normal, ahora es computed para reactividad correcta
const filteredUsers = computed(() => {
  return users.value.filter(u =>
    u.nombre?.toLowerCase().includes(searchQuery.value.toLowerCase()) ||
    u.email?.toLowerCase().includes(searchQuery.value.toLowerCase())
  );
});

const adminCount = computed(() =>
  users.value.filter(u => getRoleName(u.rolId) === 'Administrador').length
);
</script>

<template>
  <div class="flex min-h-screen bg-[#fcfcfd]">
    <Sidebar />

    <main class="flex-1 p-8 max-w-7xl mx-auto overflow-y-auto no-scrollbar animate-in fade-in slide-in-from-bottom-4 duration-700">
      <div class="flex flex-col md:flex-row md:items-center justify-between gap-6 mb-12">
        <div>
          <h1 class="text-4xl font-black tracking-tighter text-slate-900 mb-2">Usuarios</h1>
          <p class="text-slate-500 font-medium">Auditoría de perfiles y niveles de seguridad de planta — WEG Equipamientos Eléctricos S.A.</p>
        </div>
        <button class="flex items-center gap-2 bg-[#005792] text-white px-8 py-4 rounded-2xl font-black text-sm uppercase tracking-widest hover:bg-[#004a7c] transition-all shadow-xl shadow-blue-900/20 active:scale-95">
          <UserPlus class="h-5 w-5" />
          Alta de Agente
        </button>
      </div>

      <!-- Stats Row -->
      <div class="grid grid-cols-1 md:grid-cols-3 gap-6 mb-12">
        <div class="bg-white p-8 rounded-3xl shadow-sm border border-slate-100 flex items-center gap-6">
          <div class="h-16 w-16 rounded-2xl bg-blue-50 flex items-center justify-center">
            <ShieldUser class="h-8 w-8 text-[#005792]" />
          </div>
          <div>
            <span class="block text-[10px] uppercase font-black text-slate-400 tracking-widest mb-1">Total Usuarios</span>
            <span class="text-3xl font-black text-slate-900">{{ users.length }}</span>
          </div>
        </div>
        <div class="bg-white p-8 rounded-3xl shadow-sm border border-slate-100 flex items-center gap-6">
          <div class="h-16 w-16 rounded-2xl bg-emerald-50 flex items-center justify-center">
            <ShieldCheck class="h-8 w-8 text-emerald-600" />
          </div>
          <div>
            <span class="block text-[10px] uppercase font-black text-slate-400 tracking-widest mb-1">Administradores</span>
            <span class="text-3xl font-black text-slate-900">{{ adminCount }}</span>
          </div>
        </div>
      </div>

      <!-- Search -->
      <div class="bg-white p-6 rounded-3xl shadow-sm border border-slate-100 mb-8">
        <div class="relative max-w-md">
          <Search class="absolute left-4 top-1/2 -translate-y-1/2 h-5 w-5 text-slate-400" />
          <input
            v-model="searchQuery"
            type="text"
            placeholder="Buscar usuario por nombre o email..."
            class="w-full pl-12 pr-6 py-4 bg-slate-50 border-none rounded-2xl text-sm font-medium focus:ring-2 focus:ring-[#005792] transition-all outline-none"
          />
        </div>
      </div>

      <!-- Users Table -->
      <div class="bg-white rounded-3xl border border-slate-100 overflow-hidden shadow-sm">
        <div v-if="loading" class="flex flex-col items-center justify-center py-20">
          <Loader2 class="h-12 w-12 animate-spin text-[#005792]" />
          <p class="mt-4 text-slate-500 font-bold uppercase tracking-widest text-xs">Cargando personal...</p>
        </div>

        <div v-else class="overflow-x-auto">
          <table class="w-full text-left">
            <thead>
              <tr class="bg-slate-50/50">
                <th class="py-6 px-8 text-[10px] font-black uppercase tracking-widest text-slate-400">Nombre</th>
                <th class="py-6 px-4 text-[10px] font-black uppercase tracking-widest text-slate-400">Email</th>
                <th class="py-6 px-4 text-[10px] font-black uppercase tracking-widest text-slate-400">Rol</th>
                <th class="px-8 py-6 text-[10px] font-black uppercase tracking-widest text-slate-400 text-right">Acciones</th>
              </tr>
            </thead>
            <tbody class="divide-y divide-slate-50">
              <tr v-for="user in filteredUsers" :key="user.usuarioId" class="hover:bg-slate-50/30 transition-colors group">
                <td class="px-8 py-6">
                  <div class="flex items-center gap-4">
                    <div class="h-12 w-12 rounded-xl bg-slate-100 flex items-center justify-center">
                      <UserCircle class="h-6 w-6 text-slate-400" />
                    </div>
                    <div>
                      <span class="block text-sm font-black text-slate-900 group-hover:text-[#005792] transition-colors">{{ user.nombre }}</span>
                      <span class="text-xs font-bold text-slate-400 italic">ID: {{ user.usuarioId }}</span>
                    </div>
                  </div>
                </td>
                <td class="px-4 py-6">
                  <span class="text-sm font-medium text-slate-700">{{ user.email }}</span>
                </td>
                <td class="px-4 py-6">
                  <span
                    :class="[
                      'px-3 py-1.5 rounded-lg text-[10px] font-black uppercase tracking-wider',
                      getRoleName(user.rolId) === 'Administrador' ? 'bg-amber-100 text-amber-700' : 'bg-blue-100 text-blue-700'
                    ]"
                  >
                    {{ getRoleName(user.rolId) }}
                  </span>
                </td>
                <td class="px-8 py-6 text-right">
                  <div class="flex items-center justify-end gap-2 opacity-0 group-hover:opacity-100 transition-opacity">
                    <button class="p-2.5 text-slate-400 hover:text-[#005792] hover:bg-white rounded-xl shadow-sm transition-all border border-transparent hover:border-slate-100">
                      <Edit class="h-5 w-5" />
                    </button>
                    <button class="p-2.5 text-slate-400 hover:text-rose-600 hover:bg-white rounded-xl shadow-sm transition-all border border-transparent hover:border-slate-100">
                      <Trash2 class="h-5 w-5" />
                    </button>
                  </div>
                </td>
              </tr>

              <tr v-if="filteredUsers.length === 0 && !loading">
                <td colspan="4" class="py-20 text-center text-slate-300">
                  <p class="text-xs font-black uppercase tracking-widest">No se encontraron usuarios</p>
                </td>
              </tr>
            </tbody>
          </table>
        </div>
      </div>
    </main>
  </div>
</template>