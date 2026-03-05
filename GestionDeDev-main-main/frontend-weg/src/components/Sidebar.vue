<script setup lang="ts">
import { ref, watch } from "vue";
import { useRouter, useRoute } from "vue-router";
import { useAuth } from "../store/auth";
import {
  LayoutDashboard,
  Package,
  Users,
  LogOut,
  ChevronRight,
  User,
  Menu,
  X,
  Factory,
  BarChart3,
  ShieldAlert,
  Database
} from "lucide-vue-next";

const router = useRouter();
const route = useRoute();
const auth = useAuth();
const isSidebarOpen = ref(true);
const isMobileMenuOpen = ref(false);

const toggleSidebar = () => {
  isSidebarOpen.value = !isSidebarOpen.value;
};

const toggleMobileMenu = () => {
  isMobileMenuOpen.value = !isMobileMenuOpen.value;
};

watch(() => route.path, () => {
  isMobileMenuOpen.value = false;
});
</script>

<template>
  <div>
    <!-- Mobile Header -->
    <div class="lg:hidden fixed top-0 left-0 right-0 h-12 bg-white border-b border-slate-100 px-4 flex items-center justify-between z-50">
      <div class="flex items-center gap-2">
        <div class="flex h-7 w-7 items-center justify-center rounded-md bg-[#005792]">
          <Factory class="h-4 w-4 text-white" />
        </div>
        <span class="text-sm font-bold tracking-tight text-[#005792]">WEG</span>
      </div>
      <button @click="toggleMobileMenu" class="p-1.5 text-slate-500">
        <Menu v-if="!isMobileMenuOpen" class="h-5 w-5" />
        <X v-else class="h-5 w-5" />
      </button>
    </div>

    <!-- Sidebar Overlay (Mobile) -->
    <div
      v-if="isMobileMenuOpen"
      @click="isMobileMenuOpen = false"
      class="fixed inset-0 bg-slate-900/30 z-40 lg:hidden"
    ></div>

    <!-- SIDEBAR -->
    <aside
      :class="[
        'fixed inset-y-0 left-0 z-40 flex flex-col bg-white border-r border-slate-100 transition-all duration-300',
        isMobileMenuOpen ? 'translate-x-0 w-56' : '-translate-x-full lg:translate-x-0',
        !isMobileMenuOpen && (isSidebarOpen ? 'w-56' : 'w-16')
      ]"
    >
      <!-- Logo -->
      <div class="flex h-14 items-center px-4 border-b border-slate-50">
        <div class="flex items-center gap-2.5">
          <div class="flex h-8 w-8 min-w-[32px] items-center justify-center rounded-lg bg-[#005792]">
            <Factory class="h-4 w-4 text-white" />
          </div>
          <div v-if="isSidebarOpen" class="flex flex-col">
            <span class="text-sm font-bold leading-none text-[#005792]">WEG S.A.</span>
            <span class="text-[9px] font-medium text-slate-400 mt-0.5">Gestión de Devoluciones</span>
          </div>
        </div>
      </div>

      <!-- Navigation -->
      <nav class="flex-1 px-2 py-3 space-y-0.5">
        <router-link
          to="/"
          class="flex items-center gap-3 rounded-lg px-3 py-2 text-sm transition-colors"
          :class="[
            route.path === '/'
              ? 'bg-[#005792] text-white'
              : 'text-slate-600 hover:bg-slate-50 hover:text-slate-900'
          ]"
        >
          <LayoutDashboard class="h-4 w-4 min-w-[16px]" />
          <span v-if="isSidebarOpen" class="font-medium">Dashboard</span>
        </router-link>

        <router-link
          to="/returns"
          class="flex items-center gap-3 rounded-lg px-3 py-2 text-sm transition-colors"
          :class="[
            route.path.startsWith('/returns')
              ? 'bg-[#005792] text-white'
              : 'text-slate-600 hover:bg-slate-50 hover:text-slate-900'
          ]"
        >
          <Package class="h-4 w-4 min-w-[16px]" />
          <span v-if="isSidebarOpen" class="font-medium">Devoluciones</span>
        </router-link>

        <router-link
          to="/clients"
          class="flex items-center gap-3 rounded-lg px-3 py-2 text-sm transition-colors"
          :class="[
            route.path.startsWith('/clients')
              ? 'bg-[#005792] text-white'
              : 'text-slate-600 hover:bg-slate-50 hover:text-slate-900'
          ]"
        >
          <Users class="h-4 w-4 min-w-[16px]" />
          <span v-if="isSidebarOpen" class="font-medium">Clientes</span>
        </router-link>

        <router-link
          to="/products"
          class="flex items-center gap-3 rounded-lg px-3 py-2 text-sm transition-colors"
          :class="[
            route.path.startsWith('/products')
              ? 'bg-[#005792] text-white'
              : 'text-slate-600 hover:bg-slate-50 hover:text-slate-900'
          ]"
        >
          <Database class="h-4 w-4 min-w-[16px]" />
          <span v-if="isSidebarOpen" class="font-medium">Fichas Técnicas</span>
        </router-link>

        <router-link
          v-if="auth.user.value?.role === 'Administrador'"
          to="/users"
          class="flex items-center gap-3 rounded-lg px-3 py-2 text-sm transition-colors"
          :class="[
            route.path.startsWith('/users')
              ? 'bg-[#005792] text-white'
              : 'text-slate-600 hover:bg-slate-50 hover:text-slate-900'
          ]"
        >
          <ShieldAlert class="h-4 w-4 min-w-[16px]" />
          <span v-if="isSidebarOpen" class="font-medium">Usuarios</span>
        </router-link>

        <router-link
          v-if="auth.user.value?.role === 'Administrador'"
          to="/reports"
          class="flex items-center gap-3 rounded-lg px-3 py-2 text-sm transition-colors"
          :class="[
            route.path.startsWith('/reports')
              ? 'bg-[#005792] text-white'
              : 'text-slate-600 hover:bg-slate-50 hover:text-slate-900'
          ]"
        >
          <BarChart3 class="h-4 w-4 min-w-[16px]" />
          <span v-if="isSidebarOpen" class="font-medium">Reportes</span>
        </router-link>
      </nav>

      <!-- User Section -->
      <div class="border-t border-slate-100 p-3">
        <div
          :class="[
            'flex items-center gap-2.5 rounded-lg p-2',
            !isSidebarOpen && 'justify-center',
          ]"
        >
          <div class="flex h-8 w-8 min-w-[32px] items-center justify-center rounded-lg bg-slate-100">
            <User class="h-4 w-4 text-slate-500" />
          </div>
          <div v-if="isSidebarOpen" class="flex-1 overflow-hidden">
            <p class="truncate text-xs font-semibold text-slate-800">
              {{ auth.user.value?.name || "Usuario" }}
            </p>
            <p class="truncate text-[10px] text-slate-400">
              {{ auth.user.value?.role || "Operador" }}
            </p>
          </div>
          <button
            v-if="isSidebarOpen"
            @click="auth.logout(); router.push('/login');"
            class="p-1.5 rounded-md text-slate-400 hover:bg-red-50 hover:text-red-500 transition-colors"
          >
            <LogOut class="h-4 w-4" />
          </button>
        </div>

        <!-- Desktop Collapse -->
        <button
          @click="toggleSidebar"
          class="mt-1 hidden w-full items-center justify-center gap-2 rounded-lg py-2 text-[10px] font-semibold uppercase tracking-widest text-slate-400 hover:text-slate-600 hover:bg-slate-50 transition-all lg:flex"
        >
          <ChevronRight
            :class="[
              'h-4 w-4 transition-transform duration-300',
              isSidebarOpen ? 'rotate-180' : '',
            ]"
          />
          <span v-if="isSidebarOpen">Contraer</span>
        </button>
      </div>
    </aside>

    <!-- Spacer Desktop -->
    <div
      :class="[
        'hidden transition-all duration-300 lg:block',
        isSidebarOpen ? 'w-56' : 'w-16',
      ]"
    ></div>

    <!-- Padding for Mobile Header -->
    <div class="h-12 lg:hidden"></div>
  </div>
</template>