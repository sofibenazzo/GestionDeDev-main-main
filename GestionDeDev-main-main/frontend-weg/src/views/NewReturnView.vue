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
  Factory,
  Zap,
  Upload,
  FileText,
} from "lucide-vue-next";

const router = useRouter();
const clients = ref<any[]>([]);
const loading = ref(false);
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
      cv: null,
      rpm: null,
      voltaje: "",
    },
  ],
});

const archivoRemito = ref<File | null>(null);
const archivoNombre = ref("");

onMounted(async () => {
  try {
    const response = await api.get("/api/Cliente");
    clients.value = response.data;
  } catch (err) {
    console.error("Error fetching clients:", err);
    error.value = "No se pudieron cargar los clientes";
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

const { user } = useAuth();

const submitForm = async () => {
  if (!form.value.clienteId) {
    alert("Por favor seleccione un cliente");
    return;
  }

  const usuarioId = user.value?.usuarioId;
  if (!usuarioId) {
    alert(
      "Error de sesión: No se pudo identificar al usuario. Por favor reingrese."
    );
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
      usuarioId: usuarioId,
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
    router.push("/returns");
  } catch (err: any) {
    console.error("❌ ERROR AL CREAR REMITO:", err);
    if (err.response?.data?.errors) {
      const validationErrors = Object.values(err.response.data.errors)
        .flat()
        .join(", ");
      error.value = `Error de validación: ${validationErrors}`;
    } else {
      error.value =
        err.response?.data?.message ||
        err.response?.data?.error ||
        "Error al conectar con el servidor";
    }
  } finally {
    loading.value = false;
  }
};
</script>

<template>
  <div class="flex min-h-screen bg-[#f8f9fa]">
    <Sidebar />

    <main class="flex-1 p-5 lg:p-7 overflow-y-auto">
      <!-- Header -->
      <header class="mb-5">
        <button
          @click="router.back()"
          class="mb-3 flex items-center gap-1.5 text-slate-400 hover:text-slate-700 transition-colors text-xs font-medium"
        >
          <ChevronLeft class="h-3.5 w-3.5" />
          Volver al listado
        </button>
        <div class="flex items-center justify-between">
          <div>
            <h1 class="text-xl font-bold text-slate-800">Nuevo Remito</h1>
            <p class="text-xs text-slate-400 mt-0.5">
              Control de Calidad — WEG Equipamientos Eléctricos S.A.
            </p>
          </div>
          <button
            @click="submitForm"
            :disabled="loading"
            class="flex items-center gap-2 rounded-lg bg-[#005792] px-4 py-2 text-sm font-semibold text-white hover:bg-blue-700 transition-colors disabled:opacity-50"
          >
            <Save class="h-4 w-4" />
            <span v-if="loading">Guardando...</span>
            <span v-else>Guardar</span>
          </button>
        </div>
      </header>

      <!-- Error -->
      <div
        v-if="error"
        class="mb-4 flex items-start gap-2.5 rounded-lg border border-red-100 bg-red-50 p-3"
      >
        <AlertCircle class="h-4 w-4 text-red-500 mt-0.5 shrink-0" />
        <p class="text-xs text-red-600">{{ error }}</p>
      </div>

      <div class="grid grid-cols-1 lg:grid-cols-3 gap-5">
        <!-- Main Form -->
        <div class="lg:col-span-2 space-y-4">
          <!-- Client Selection -->
          <section class="bg-white rounded-xl border border-slate-100 p-5">
            <h2
              class="mb-4 flex items-center gap-2 text-sm font-semibold text-slate-700"
            >
              <User class="h-4 w-4 text-[#005792]" />
              Información del Cliente
            </h2>
            <div>
              <label
                class="block text-[10px] font-semibold uppercase tracking-wider text-slate-400 mb-1.5"
                >Selección de Cliente</label
              >
              <select
                v-model="form.clienteId"
                class="w-full rounded-lg border border-slate-200 bg-white px-3 py-2 text-sm text-slate-800 focus:border-[#005792] focus:outline-none focus:ring-2 focus:ring-blue-900/10"
              >
                <option value="" disabled>
                  Seleccione una Razón Social...
                </option>
                <option
                  v-for="client in clients"
                  :key="client.clienteId"
                  :value="client.clienteId"
                >
                  {{ client.razonSocial }} ({{ client.codigoCliente }})
                </option>
              </select>
            </div>
          </section>

          <!-- Products -->
          <section class="bg-white rounded-xl border border-slate-100 p-5">
            <div class="flex items-center justify-between mb-4">
              <h2
                class="flex items-center gap-2 text-sm font-semibold text-slate-700"
              >
                <Package class="h-4 w-4 text-[#005792]" />
                Detalle de Productos
              </h2>
              <button
                @click="addProduct"
                class="flex items-center gap-1.5 rounded-lg border border-slate-200 px-3 py-1.5 text-xs font-medium text-slate-600 hover:border-[#005792] hover:text-[#005792] transition-colors"
              >
                <Plus class="h-3.5 w-3.5" />
                Agregar renglón
              </button>
            </div>

            <div class="space-y-3">
              <div
                v-for="(prod, index) in form.productos"
                :key="index"
                class="relative rounded-lg border border-slate-100 bg-slate-50/50 p-4"
              >
                <div class="grid grid-cols-2 md:grid-cols-4 gap-3">
                  <div class="md:col-span-2">
                    <label
                      class="block text-[10px] font-semibold uppercase tracking-wider text-slate-400 mb-1"
                      >Designación</label
                    >
                    <input
                      v-model="prod.item"
                      placeholder="Ej. Motor Trifásico"
                      class="w-full rounded-md border border-slate-200 bg-white px-3 py-1.5 text-sm text-slate-800 focus:border-[#005792] focus:outline-none"
                    />
                  </div>
                  <div>
                    <label
                      class="block text-[10px] font-semibold uppercase tracking-wider text-slate-400 mb-1"
                      >Modelo</label
                    >
                    <input
                      v-model="prod.modelo"
                      placeholder="Cód. Modelo"
                      class="w-full rounded-md border border-slate-200 bg-white px-3 py-1.5 text-sm text-slate-800 focus:border-[#005792] focus:outline-none"
                    />
                  </div>
                  <div>
                    <label
                      class="block text-[10px] font-semibold uppercase tracking-wider text-slate-400 mb-1"
                      >Cant.</label
                    >
                    <input
                      v-model="prod.cantidad"
                      type="number"
                      min="1"
                      class="w-full rounded-md border border-slate-200 bg-white px-3 py-1.5 text-sm text-slate-800 focus:border-[#005792] focus:outline-none text-center"
                    />
                  </div>
                  <div>
                    <label
                      class="block text-[10px] font-semibold uppercase tracking-wider text-slate-400 mb-1"
                      >N° Serie</label
                    >
                    <input
                      v-model="prod.numeroSerie"
                      placeholder="Serial"
                      class="w-full rounded-md border border-slate-200 bg-white px-3 py-1.5 text-sm text-slate-800 focus:border-[#005792] focus:outline-none"
                    />
                  </div>
                  <div>
                    <label
                      class="block text-[10px] font-semibold uppercase tracking-wider text-slate-400 mb-1"
                      >CV</label
                    >
                    <input
                      v-model="prod.cv"
                      type="number"
                      step="0.1"
                      placeholder="Potencia"
                      class="w-full rounded-md border border-slate-200 bg-white px-3 py-1.5 text-sm text-slate-800 focus:border-[#005792] focus:outline-none"
                    />
                  </div>
                  <div>
                    <label
                      class="block text-[10px] font-semibold uppercase tracking-wider text-slate-400 mb-1"
                      >RPM</label
                    >
                    <input
                      v-model="prod.rpm"
                      type="number"
                      placeholder="Velocidad"
                      class="w-full rounded-md border border-slate-200 bg-white px-3 py-1.5 text-sm text-slate-800 focus:border-[#005792] focus:outline-none"
                    />
                  </div>
                  <div>
                    <label
                      class="block text-[10px] font-semibold uppercase tracking-wider text-slate-400 mb-1"
                      >Voltaje</label
                    >
                    <input
                      v-model="prod.voltaje"
                      placeholder="220/380V"
                      class="w-full rounded-md border border-slate-200 bg-white px-3 py-1.5 text-sm text-slate-800 focus:border-[#005792] focus:outline-none"
                    />
                  </div>
                </div>

                <button
                  v-if="form.productos.length > 1"
                  @click="removeProduct(index)"
                  class="absolute top-2 right-2 h-6 w-6 rounded-md bg-white border border-slate-100 text-slate-400 flex items-center justify-center hover:border-red-200 hover:text-red-500 transition-colors"
                >
                  <Trash2 class="h-3.5 w-3.5" />
                </button>
              </div>
            </div>
          </section>

          <!-- Motivo y Observaciones -->
          <section class="bg-white rounded-xl border border-slate-100 p-5">
            <h2
              class="mb-4 flex items-center gap-2 text-sm font-semibold text-slate-700"
            >
              <FileText class="h-4 w-4 text-[#005792]" />
              Motivo y Observaciones
            </h2>
            <div class="space-y-3">
              <div>
                <label
                  class="block text-[10px] font-semibold uppercase tracking-wider text-slate-400 mb-1.5"
                  >Motivo de la Devolución</label
                >
                <select
                  v-model="form.motivo"
                  class="w-full rounded-lg border border-slate-200 bg-white px-3 py-2 text-sm text-slate-800 focus:border-[#005792] focus:outline-none focus:ring-2 focus:ring-blue-900/10"
                >
                  <option value="">Seleccione un motivo...</option>
                  <option value="Defecto de fábrica">Defecto de fábrica</option>
                  <option value="Producto dañado en transporte">
                    Producto dañado en transporte
                  </option>
                  <option value="Producto incorrecto">
                    Producto incorrecto
                  </option>
                  <option value="Falla en funcionamiento">
                    Falla en funcionamiento
                  </option>
                  <option value="Garantía">Garantía</option>
                  <option value="Otro">Otro</option>
                </select>
              </div>
              <div>
                <label
                  class="block text-[10px] font-semibold uppercase tracking-wider text-slate-400 mb-1.5"
                  >Observaciones</label
                >
                <textarea
                  v-model="form.observacionTexto"
                  rows="3"
                  placeholder="Detalles adicionales sobre la devolución..."
                  class="w-full rounded-lg border border-slate-200 bg-white px-3 py-2 text-sm text-slate-800 focus:border-[#005792] focus:outline-none focus:ring-2 focus:ring-blue-900/10 resize-none"
                ></textarea>
              </div>
              <div>
                <label
                  class="block text-[10px] font-semibold uppercase tracking-wider text-slate-400 mb-1.5"
                  >Archivo (PDF o imagen, opcional)</label
                >
                <input
                  type="file"
                  accept=".pdf,.png,.jpg,.jpeg"
                  @change="(e: any) => { archivoRemito = e.target.files[0]; archivoNombre = e.target.files[0]?.name || ''; }"
                  class="hidden"
                  id="file-upload"
                />
                <label
                  for="file-upload"
                  class="flex items-center gap-2 cursor-pointer rounded-lg border border-dashed border-slate-200 px-4 py-3 text-sm text-slate-400 hover:border-[#005792] hover:text-[#005792] transition-colors"
                >
                  <Upload class="h-4 w-4" />
                  <span v-if="archivoNombre" class="text-slate-700">{{
                    archivoNombre
                  }}</span>
                  <span v-else>Seleccionar archivo...</span>
                </label>
              </div>
            </div>
          </section>
        </div>

        <!-- Sidebar Info -->
        <div class="space-y-4">
          <div class="bg-[#005792] rounded-xl p-5">
            <h3
              class="flex items-center gap-2 text-xs font-semibold uppercase tracking-wider text-white/80 mb-3"
            >
              <Zap class="h-3.5 w-3.5" />
              Estado Inicial
            </h3>
            <div class="rounded-lg bg-white/10 px-4 py-3">
              <p
                class="text-[10px] text-blue-200 uppercase tracking-wider mb-0.5"
              >
                Estado
              </p>
              <p class="text-sm font-bold text-white">PENDIENTE</p>
              <p class="mt-2 text-[11px] text-blue-200/70 leading-relaxed">
                Todo registro nuevo ingresa como Pendiente hasta que un
                administrador lo revise.
              </p>
            </div>
          </div>

          <div class="bg-white rounded-xl border border-slate-100 p-5">
            <h4
              class="text-[10px] font-semibold uppercase tracking-wider text-slate-400 mb-3"
            >
              Ayuda Rápida
            </h4>
            <ul class="space-y-2">
              <li class="flex items-start gap-2">
                <div class="h-1 w-1 rounded-full bg-[#005792] mt-1.5"></div>
                <p class="text-xs text-slate-500">
                  Ingrese el número de serie para trazabilidad.
                </p>
              </li>
              <li class="flex items-start gap-2">
                <div class="h-1 w-1 rounded-full bg-[#005792] mt-1.5"></div>
                <p class="text-xs text-slate-500">
                  CV y RPM deben ser números válidos.
                </p>
              </li>
            </ul>
          </div>
        </div>
      </div>
    </main>
  </div>
</template>