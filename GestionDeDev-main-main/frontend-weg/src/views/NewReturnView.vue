<script setup lang="ts">
import { ref, onMounted } from "vue";
import { useRouter } from "vue-router";
import { useAuth } from "../store/auth";
import Sidebar from "../components/Sidebar.vue";
import api from "../services/api";
import {
  Plus,
  Trash2,
  Save,
  ChevronLeft,
  User,
  Package,
  AlertCircle,
  Zap,
  Upload,
  FileText,
  X,
  CheckCircle2,
  ClipboardList,
} from "lucide-vue-next";

const router = useRouter();
const clients = ref<any[]>([]);
const loading = ref(false);
const success = ref(false);
const error = ref<string | null>(null);

const form = ref({
  clienteId: "",
  motivo: "",
  observacionTexto: "",
  productos: [
    {
      item: "",
      modelo: "",
      cantidad: 1,
      numeroSerie: "",
      cv: null as number | null,
      rpm: null as number | null,
      voltaje: "",
    },
  ],
});

const archivoRemito = ref<File | null>(null);
const archivoNombre = ref("");
const archivoPreview = ref<string | null>(null);

onMounted(async () => {
  try {
    const response = await api.get("/api/Cliente");
    clients.value = response.data;
  } catch (err) {
    console.error("Error fetching clients:", err);
    error.value = "No se pudieron cargar los clientes.";
  }
});

const addProduct = () => {
  form.value.productos.push({
    item: "",
    modelo: "",
    cantidad: 1,
    numeroSerie: "",
    cv: null,
    rpm: null,
    voltaje: "",
  });
};

const removeProduct = (index: number) => {
  if (form.value.productos.length > 1) {
    form.value.productos.splice(index, 1);
  }
};

const handleFileChange = (e: any) => {
  const file = e.target.files[0];
  if (!file) return;
  archivoRemito.value = file;
  archivoNombre.value = file.name;
  if (file.type.startsWith("image/")) {
    const reader = new FileReader();
    reader.onload = (ev) => { archivoPreview.value = ev.target?.result as string; };
    reader.readAsDataURL(file);
  } else {
    archivoPreview.value = null;
  }
};

const removeFile = () => {
  archivoRemito.value = null;
  archivoNombre.value = "";
  archivoPreview.value = null;
  const input = document.getElementById("file-upload") as HTMLInputElement;
  if (input) input.value = "";
};

const { user } = useAuth();

const submitForm = async () => {
  if (!form.value.clienteId) {
    error.value = "Por favor seleccione un cliente para continuar.";
    return;
  }

  const usuarioId = user.value?.usuarioId;
  if (!usuarioId) {
    error.value = "Error de sesión. Por favor reingrese al sistema.";
    return;
  }

  loading.value = true;
  error.value = null;

  try {
    let archivoPath = "";
    if (archivoRemito.value) {
      const formData = new FormData();
      formData.append("archivo", archivoRemito.value);
      const uploadRes = await api.post("/api/Remito/upload", formData, {
        headers: { "Content-Type": "multipart/form-data" },
      });
      archivoPath = uploadRes.data.fileName;
    }

    const payload = {
      clienteId: parseInt(form.value.clienteId),
      usuarioId,
      fecha: new Date().toISOString(),
      tipoEstadoId: 1,
      motivo: form.value.motivo || null,
      observacionTexto: form.value.observacionTexto || null,
      archivoRemito: archivoPath || null,
      productos: form.value.productos.map((p: any) => ({
        ...p,
        cantidad: parseInt(p.cantidad.toString()),
        cv: p.cv !== null && p.cv !== undefined ? String(p.cv) : null,
        rpm: p.rpm !== null && p.rpm !== undefined ? String(p.rpm) : null,
      })),
    };

    await api.post("/api/Remito", payload);
    success.value = true;
    setTimeout(() => router.push("/returns"), 1500);
  } catch (err: any) {
    console.error("Error al crear devolución:", err);
    if (err.response?.data?.errors) {
      const validationErrors = Object.values(err.response.data.errors).flat().join(", ");
      error.value = `Error de validación: ${validationErrors}`;
    } else {
      error.value = err.response?.data?.message || err.response?.data?.error || "Error al conectar con el servidor.";
    }
  } finally {
    loading.value = false;
  }
};

const selectedClient = () => clients.value.find((c) => c.clienteId == form.value.clienteId);
</script>

<template>
  <div class="flex min-h-screen bg-[#f4f6f9]">
    <Sidebar />

    <main class="flex-1 overflow-y-auto">
      <!-- Top Bar -->
      <div class="sticky top-0 z-10 bg-white border-b border-slate-200 px-8 py-4 flex items-center justify-between shadow-sm">
        <div class="flex items-center gap-4">
          <button
            @click="router.back()"
            class="flex items-center gap-2 text-slate-400 hover:text-slate-700 transition-colors text-xs font-semibold uppercase tracking-wider"
          >
            <ChevronLeft class="h-4 w-4" />
            Volver
          </button>
          <div class="h-5 w-px bg-slate-200"></div>
          <div>
            <h1 class="text-base font-black text-slate-800 tracking-tight">Nueva Devolución</h1>
            <p class="text-[10px] text-slate-400 font-semibold uppercase tracking-widest">WEG Equipamientos Eléctricos S.A.</p>
          </div>
        </div>

        <div class="flex items-center gap-3">
          <div v-if="success" class="flex items-center gap-2 text-emerald-600 text-xs font-bold">
            <CheckCircle2 class="h-4 w-4" />
            Registrado exitosamente
          </div>
          <button
            @click="submitForm"
            :disabled="loading || success"
            class="flex items-center gap-2 rounded-lg bg-[#005792] px-6 py-2.5 text-sm font-bold text-white hover:bg-blue-700 transition-all disabled:opacity-50 shadow-md shadow-blue-900/20"
          >
            <Save class="h-4 w-4" />
            {{ loading ? "Registrando..." : "Registrar Devolución" }}
          </button>
        </div>
      </div>

      <div class="max-w-6xl mx-auto px-8 py-8">
        <!-- Error Banner -->
        <div
          v-if="error"
          class="mb-6 flex items-start gap-3 rounded-xl border border-red-200 bg-red-50 px-5 py-4"
        >
          <AlertCircle class="h-5 w-5 text-red-500 mt-0.5 shrink-0" />
          <div>
            <p class="text-sm font-bold text-red-700">Error al procesar</p>
            <p class="text-xs text-red-500 mt-0.5">{{ error }}</p>
          </div>
          <button @click="error = null" class="ml-auto text-red-400 hover:text-red-600">
            <X class="h-4 w-4" />
          </button>
        </div>

        <div class="grid grid-cols-1 lg:grid-cols-3 gap-6">
          <!-- Columna principal -->
          <div class="lg:col-span-2 space-y-6">

            <!-- SECCIÓN 1: Cliente -->
            <section class="bg-white rounded-2xl border border-slate-200 overflow-hidden shadow-sm">
              <div class="px-6 py-4 border-b border-slate-100 flex items-center gap-3">
                <div class="h-8 w-8 rounded-lg bg-blue-50 flex items-center justify-center">
                  <User class="h-4 w-4 text-[#005792]" />
                </div>
                <div>
                  <h2 class="text-sm font-black text-slate-800">Cliente</h2>
                  <p class="text-[10px] text-slate-400 font-semibold uppercase tracking-wider">Entidad solicitante de la devolución</p>
                </div>
              </div>
              <div class="px-6 py-5">
                <label class="block text-[10px] font-black uppercase tracking-widest text-slate-500 mb-2">
                  Razón Social <span class="text-red-400">*</span>
                </label>
                <select
                  v-model="form.clienteId"
                  class="w-full rounded-xl border border-slate-200 bg-slate-50 px-4 py-3 text-sm font-semibold text-slate-800 focus:border-[#005792] focus:bg-white focus:outline-none focus:ring-3 focus:ring-blue-500/10 transition-all"
                >
                  <option value="" disabled>Seleccione una Razón Social...</option>
                  <option
                    v-for="client in clients"
                    :key="client.clienteId"
                    :value="client.clienteId"
                  >
                    {{ client.razonSocial }}{{ client.codigoCliente ? ` — ${client.codigoCliente}` : '' }}
                  </option>
                </select>

                <!-- Info del cliente seleccionado -->
                <div v-if="selectedClient()" class="mt-4 p-4 rounded-xl bg-blue-50 border border-blue-100 flex items-center gap-3">
                  <div class="h-10 w-10 rounded-lg bg-[#005792] flex items-center justify-center shrink-0">
                    <User class="h-5 w-5 text-white" />
                  </div>
                  <div>
                    <p class="text-sm font-black text-slate-800">{{ selectedClient()?.razonSocial }}</p>
                    <p class="text-[10px] text-slate-500 font-semibold">
                      {{ selectedClient()?.codigoCliente || 'Sin código' }} 
                      · {{ selectedClient()?.direccion?.ciudad || 'Sin localidad' }}
                    </p>
                  </div>
                </div>
              </div>
            </section>

            <!-- SECCIÓN 2: Productos -->
            <section class="bg-white rounded-2xl border border-slate-200 overflow-hidden shadow-sm">
              <div class="px-6 py-4 border-b border-slate-100 flex items-center justify-between">
                <div class="flex items-center gap-3">
                  <div class="h-8 w-8 rounded-lg bg-blue-50 flex items-center justify-center">
                    <Package class="h-4 w-4 text-[#005792]" />
                  </div>
                  <div>
                    <h2 class="text-sm font-black text-slate-800">Productos a Devolver</h2>
                    <p class="text-[10px] text-slate-400 font-semibold uppercase tracking-wider">{{ form.productos.length }} ítem(s) en esta devolución</p>
                  </div>
                </div>
                <button
                  @click="addProduct"
                  class="flex items-center gap-1.5 rounded-lg bg-slate-900 px-4 py-2 text-xs font-bold text-white hover:bg-[#005792] transition-colors"
                >
                  <Plus class="h-3.5 w-3.5" />
                  Agregar ítem
                </button>
              </div>

              <div class="divide-y divide-slate-100">
                <div
                  v-for="(prod, index) in form.productos"
                  :key="index"
                  class="px-6 py-5 relative"
                >
                  <!-- Número de ítem -->
                  <div class="flex items-center gap-2 mb-4">
                    <span class="inline-flex h-6 w-6 items-center justify-center rounded-full bg-slate-900 text-white text-[10px] font-black">{{ index + 1 }}</span>
                    <span class="text-xs font-bold text-slate-500 uppercase tracking-wider">Ítem {{ index + 1 }}</span>
                    <button
                      v-if="form.productos.length > 1"
                      @click="removeProduct(index)"
                      class="ml-auto flex items-center gap-1 text-[10px] font-bold text-slate-400 hover:text-red-500 transition-colors"
                    >
                      <Trash2 class="h-3.5 w-3.5" />
                      Eliminar
                    </button>
                  </div>

                  <div class="grid grid-cols-2 md:grid-cols-4 gap-4">
                    <div class="md:col-span-2">
                      <label class="block text-[10px] font-black uppercase tracking-widest text-slate-500 mb-1.5">Designación del Producto</label>
                      <input
                        v-model="prod.item"
                        placeholder="Ej: Motor Trifásico IE3"
                        class="w-full rounded-lg border border-slate-200 bg-slate-50 px-3 py-2.5 text-sm text-slate-800 focus:border-[#005792] focus:bg-white focus:outline-none transition-all"
                      />
                    </div>
                    <div>
                      <label class="block text-[10px] font-black uppercase tracking-widest text-slate-500 mb-1.5">Modelo</label>
                      <input
                        v-model="prod.modelo"
                        placeholder="Cód. Modelo"
                        class="w-full rounded-lg border border-slate-200 bg-slate-50 px-3 py-2.5 text-sm text-slate-800 focus:border-[#005792] focus:bg-white focus:outline-none transition-all"
                      />
                    </div>
                    <div>
                      <label class="block text-[10px] font-black uppercase tracking-widest text-slate-500 mb-1.5">Cantidad</label>
                      <input
                        v-model="prod.cantidad"
                        type="number"
                        min="1"
                        class="w-full rounded-lg border border-slate-200 bg-slate-50 px-3 py-2.5 text-sm text-slate-800 focus:border-[#005792] focus:bg-white focus:outline-none transition-all text-center font-bold"
                      />
                    </div>
                    <div>
                      <label class="block text-[10px] font-black uppercase tracking-widest text-slate-500 mb-1.5">N° de Serie</label>
                      <input
                        v-model="prod.numeroSerie"
                        placeholder="Serial"
                        class="w-full rounded-lg border border-slate-200 bg-slate-50 px-3 py-2.5 text-sm text-slate-800 focus:border-[#005792] focus:bg-white focus:outline-none transition-all"
                      />
                    </div>
                    <div>
                      <label class="block text-[10px] font-black uppercase tracking-widest text-slate-500 mb-1.5">Potencia (CV)</label>
                      <input
                        v-model="prod.cv"
                        type="number"
                        step="0.1"
                        placeholder="—"
                        class="w-full rounded-lg border border-slate-200 bg-slate-50 px-3 py-2.5 text-sm text-slate-800 focus:border-[#005792] focus:bg-white focus:outline-none transition-all"
                      />
                    </div>
                    <div>
                      <label class="block text-[10px] font-black uppercase tracking-widest text-slate-500 mb-1.5">Velocidad (RPM)</label>
                      <input
                        v-model="prod.rpm"
                        type="number"
                        placeholder="—"
                        class="w-full rounded-lg border border-slate-200 bg-slate-50 px-3 py-2.5 text-sm text-slate-800 focus:border-[#005792] focus:bg-white focus:outline-none transition-all"
                      />
                    </div>
                    <div>
                      <label class="block text-[10px] font-black uppercase tracking-widest text-slate-500 mb-1.5">Voltaje</label>
                      <input
                        v-model="prod.voltaje"
                        placeholder="220/380V"
                        class="w-full rounded-lg border border-slate-200 bg-slate-50 px-3 py-2.5 text-sm text-slate-800 focus:border-[#005792] focus:bg-white focus:outline-none transition-all"
                      />
                    </div>
                  </div>
                </div>
              </div>
            </section>

            <!-- SECCIÓN 3: Motivo y Observaciones -->
            <section class="bg-white rounded-2xl border border-slate-200 overflow-hidden shadow-sm">
              <div class="px-6 py-4 border-b border-slate-100 flex items-center gap-3">
                <div class="h-8 w-8 rounded-lg bg-blue-50 flex items-center justify-center">
                  <ClipboardList class="h-4 w-4 text-[#005792]" />
                </div>
                <div>
                  <h2 class="text-sm font-black text-slate-800">Motivo y Observaciones</h2>
                  <p class="text-[10px] text-slate-400 font-semibold uppercase tracking-wider">Descripción del caso</p>
                </div>
              </div>
              <div class="px-6 py-5 space-y-5">
                <div>
                  <label class="block text-[10px] font-black uppercase tracking-widest text-slate-500 mb-2">Motivo de la Devolución</label>
                  <select
                    v-model="form.motivo"
                    class="w-full rounded-xl border border-slate-200 bg-slate-50 px-4 py-3 text-sm font-semibold text-slate-800 focus:border-[#005792] focus:bg-white focus:outline-none focus:ring-3 focus:ring-blue-500/10 transition-all"
                  >
                    <option value="">Seleccione un motivo...</option>
                    <option value="Defecto de fábrica">Defecto de fábrica</option>
                    <option value="Producto dañado en transporte">Producto dañado en transporte</option>
                    <option value="Producto incorrecto">Producto incorrecto</option>
                    <option value="Falla en funcionamiento">Falla en funcionamiento</option>
                    <option value="Garantía">Garantía</option>
                    <option value="Otro">Otro</option>
                  </select>
                </div>

                <div>
                  <label class="block text-[10px] font-black uppercase tracking-widest text-slate-500 mb-2">Observaciones Adicionales</label>
                  <textarea
                    v-model="form.observacionTexto"
                    rows="4"
                    placeholder="Describa el problema, condición del producto u observaciones relevantes para el área técnica..."
                    class="w-full rounded-xl border border-slate-200 bg-slate-50 px-4 py-3 text-sm text-slate-800 focus:border-[#005792] focus:bg-white focus:outline-none focus:ring-3 focus:ring-blue-500/10 transition-all resize-none"
                  ></textarea>
                </div>

                <!-- ARCHIVO ADJUNTO - zona cómoda y clara -->
                <div>
                  <label class="block text-[10px] font-black uppercase tracking-widest text-slate-500 mb-2">
                    Documentación Adjunta
                    <span class="ml-2 normal-case font-semibold text-slate-400">(opcional — PDF, PNG, JPG)</span>
                  </label>

                  <!-- Sin archivo: zona de drop -->
                  <div v-if="!archivoNombre">
                    <input
                      type="file"
                      accept=".pdf,.png,.jpg,.jpeg"
                      @change="handleFileChange"
                      class="hidden"
                      id="file-upload"
                    />
                    <label
                      for="file-upload"
                      class="flex flex-col items-center justify-center gap-3 cursor-pointer rounded-xl border-2 border-dashed border-slate-200 bg-slate-50 px-6 py-8 text-center hover:border-[#005792] hover:bg-blue-50/30 transition-all group"
                    >
                      <div class="h-12 w-12 rounded-xl bg-white shadow-sm border border-slate-200 flex items-center justify-center group-hover:border-[#005792] transition-colors">
                        <Upload class="h-6 w-6 text-slate-400 group-hover:text-[#005792] transition-colors" />
                      </div>
                      <div>
                        <p class="text-sm font-bold text-slate-600 group-hover:text-[#005792] transition-colors">Adjuntar remito u otro documento</p>
                        <p class="text-xs text-slate-400 mt-1">Haga clic para seleccionar o arrastre el archivo aquí</p>
                        <p class="text-[10px] text-slate-300 mt-1 font-semibold uppercase tracking-wider">PDF · PNG · JPG — Máx. 10MB</p>
                      </div>
                    </label>
                  </div>

                  <!-- Con archivo: preview + opción de eliminar -->
                  <div v-else class="rounded-xl border border-slate-200 bg-white overflow-hidden">
                    <!-- Preview imagen -->
                    <div v-if="archivoPreview" class="relative">
                      <img :src="archivoPreview" alt="Preview" class="w-full max-h-48 object-cover" />
                      <div class="absolute inset-0 bg-gradient-to-t from-black/40 to-transparent"></div>
                    </div>

                    <div class="flex items-center gap-4 px-4 py-3">
                      <div class="h-10 w-10 rounded-lg flex items-center justify-center shrink-0"
                        :class="archivoPreview ? 'bg-emerald-50' : 'bg-red-50'">
                        <FileText :class="archivoPreview ? 'text-emerald-600' : 'text-red-500'" class="h-5 w-5" />
                      </div>
                      <div class="flex-1 min-w-0">
                        <p class="text-sm font-bold text-slate-800 truncate">{{ archivoNombre }}</p>
                        <p class="text-[10px] text-slate-400 font-semibold">Archivo adjunto · Listo para enviar</p>
                      </div>
                      <button
                        @click="removeFile"
                        class="flex items-center gap-1.5 rounded-lg border border-red-100 bg-red-50 px-3 py-1.5 text-xs font-bold text-red-500 hover:bg-red-100 transition-colors"
                      >
                        <X class="h-3.5 w-3.5" />
                        Quitar
                      </button>
                    </div>
                  </div>
                </div>
              </div>
            </section>
          </div>

          <!-- Columna lateral -->
          <div class="space-y-5">
            <!-- Estado inicial -->
            <div class="bg-[#005792] rounded-2xl p-6 shadow-xl shadow-blue-900/20">
              <div class="flex items-center gap-2 mb-4">
                <Zap class="h-4 w-4 text-blue-300" />
                <h3 class="text-xs font-black uppercase tracking-widest text-blue-200">Estado Inicial</h3>
              </div>
              <div class="bg-white/10 rounded-xl px-4 py-4 mb-4">
                <p class="text-[10px] text-blue-300 uppercase tracking-widest font-bold mb-1">Estado del Registro</p>
                <p class="text-xl font-black text-white tracking-tight">PENDIENTE</p>
                <p class="mt-2 text-[11px] text-blue-200/70 leading-relaxed">
                  El registro ingresa como Pendiente y será revisado por el área técnica o administrativa.
                </p>
              </div>
              <div class="space-y-2">
                <div class="flex items-center gap-2 text-[10px] font-semibold text-blue-200">
                  <div class="h-1.5 w-1.5 rounded-full bg-emerald-400"></div>
                  Notificación automática al equipo
                </div>
                <div class="flex items-center gap-2 text-[10px] font-semibold text-blue-200">
                  <div class="h-1.5 w-1.5 rounded-full bg-emerald-400"></div>
                  Trazabilidad completa del proceso
                </div>
                <div class="flex items-center gap-2 text-[10px] font-semibold text-blue-200">
                  <div class="h-1.5 w-1.5 rounded-full bg-emerald-400"></div>
                  Resolución en menos de 72hs hábiles
                </div>
              </div>
            </div>

            <!-- Guía rápida -->
            <div class="bg-white rounded-2xl border border-slate-200 p-6 shadow-sm">
              <h4 class="text-[10px] font-black uppercase tracking-widest text-slate-500 mb-4">Guía de Registro</h4>
              <div class="space-y-4">
                <div class="flex gap-3">
                  <div class="h-6 w-6 shrink-0 flex items-center justify-center rounded-full bg-slate-900 text-white text-[10px] font-black">1</div>
                  <p class="text-xs text-slate-500 leading-relaxed font-medium">Seleccione el cliente que solicita la devolución.</p>
                </div>
                <div class="flex gap-3">
                  <div class="h-6 w-6 shrink-0 flex items-center justify-center rounded-full bg-slate-900 text-white text-[10px] font-black">2</div>
                  <p class="text-xs text-slate-500 leading-relaxed font-medium">Complete el detalle de cada producto. El N° de serie es clave para la trazabilidad.</p>
                </div>
                <div class="flex gap-3">
                  <div class="h-6 w-6 shrink-0 flex items-center justify-center rounded-full bg-slate-900 text-white text-[10px] font-black">3</div>
                  <p class="text-xs text-slate-500 leading-relaxed font-medium">Indique el motivo y adjunte documentación si dispone (remito, foto, etc.).</p>
                </div>
                <div class="flex gap-3">
                  <div class="h-6 w-6 shrink-0 flex items-center justify-center rounded-full bg-[#005792] text-white text-[10px] font-black">4</div>
                  <p class="text-xs text-slate-500 leading-relaxed font-medium">Presione <span class="font-bold text-slate-700">Registrar Devolución</span> para enviar al sistema.</p>
                </div>
              </div>
            </div>

            <!-- Resumen rápido -->
            <div v-if="form.clienteId" class="bg-slate-900 rounded-2xl p-6 text-white">
              <h4 class="text-[10px] font-black uppercase tracking-widest text-slate-400 mb-4">Resumen del Registro</h4>
              <div class="space-y-3">
                <div class="flex items-center justify-between">
                  <span class="text-xs text-slate-400 font-semibold">Cliente</span>
                  <span class="text-xs font-black text-white truncate max-w-[140px]">{{ selectedClient()?.razonSocial }}</span>
                </div>
                <div class="flex items-center justify-between">
                  <span class="text-xs text-slate-400 font-semibold">Ítems</span>
                  <span class="text-xs font-black text-white">{{ form.productos.length }}</span>
                </div>
                <div class="flex items-center justify-between">
                  <span class="text-xs text-slate-400 font-semibold">Motivo</span>
                  <span class="text-xs font-black text-white">{{ form.motivo || '—' }}</span>
                </div>
                <div class="flex items-center justify-between">
                  <span class="text-xs text-slate-400 font-semibold">Archivo</span>
                  <span class="text-xs font-black" :class="archivoNombre ? 'text-emerald-400' : 'text-slate-500'">
                    {{ archivoNombre ? 'Adjunto' : 'Sin adjunto' }}
                  </span>
                </div>
                <div class="pt-3 border-t border-slate-700 flex items-center justify-between">
                  <span class="text-xs text-slate-400 font-semibold">Estado</span>
                  <span class="inline-flex items-center rounded-full bg-amber-500/20 px-2.5 py-1 text-[10px] font-black text-amber-400 uppercase tracking-wider">Pendiente</span>
                </div>
              </div>
            </div>
          </div>
        </div>
      </div>
    </main>
  </div>
</template>