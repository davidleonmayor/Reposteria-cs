"use client";

import { useState } from "react";
import {
  LineChart,
  Line,
  XAxis,
  YAxis,
  CartesianGrid,
  Tooltip,
  ResponsiveContainer,
} from "recharts";

import {
  Tabs,
  TabsList,
  TabsTrigger,
} from "@/components/ui/tabs";

// Datos mock para ventas
const weeklyData = [
  { label: "Lun", ventas: 12 },
  { label: "Mar", ventas: 19 },
  { label: "Mié", ventas: 15 },
  { label: "Jue", ventas: 28 },
  { label: "Vie", ventas: 35 },
  { label: "Sáb", ventas: 42 },
  { label: "Dom", ventas: 38 },
];

const monthlyData = [
  { label: "Ene", ventas: 120 },
  { label: "Feb", ventas: 145 },
  { label: "Mar", ventas: 98 },
  { label: "Abr", ventas: 167 },
  { label: "May", ventas: 189 },
  { label: "Jun", ventas: 210 },
  { label: "Jul", ventas: 178 },
  { label: "Ago", ventas: 195 },
  { label: "Sep", ventas: 232 },
  { label: "Oct", ventas: 256 },
  { label: "Nov", ventas: 278 },
  { label: "Dic", ventas: 310 },
];

const yearlyData = [
  { label: "2020", ventas: 1240 },
  { label: "2021", ventas: 1580 },
  { label: "2022", ventas: 1890 },
  { label: "2023", ventas: 2150 },
  { label: "2024", ventas: 2640 },
  { label: "2025", ventas: 3100 },
];

const dataByPeriod = {
  semana: weeklyData,
  mes: monthlyData,
  año: yearlyData,
};

type Period = keyof typeof dataByPeriod;

// Datos de ventas por categoría (mock)
const categorySalesData = [
  { name: "Tortas", value: 420 },
  { name: "Panadería", value: 280 },
  { name: "Postres", value: 195 },
  { name: "Bocados", value: 150 },
  { name: "Tartas", value: 110 },
  { name: "Galletas", value: 85 },
];

const totalSales = categorySalesData.reduce((sum, item) => sum + item.value, 0);
const GREEN_COLOR = "#10b981";

// Tooltip personalizado con el estilo del proyecto
const CustomTooltip = ({
  active,
  payload,
  label,
}: {
  active?: boolean;
  payload?: Array<{ value: number }>;
  label?: string;
}) => {
  if (active && payload && payload.length) {
    return (
      <div className="rounded-lg border border-slate-200 bg-white px-4 py-3 shadow-lg">
        <p className="text-sm font-medium text-slate-600">{label}</p>
        <p className="text-lg font-bold text-emerald-600">
          {payload[0].value} ventas
        </p>
      </div>
    );
  }
  return null;
};

export default function Sells() {
  const [period, setPeriod] = useState<Period>("semana");
  const data = dataByPeriod[period];

  const tickCount = period === "mes" ? 6 : undefined;

  return (
    <main className="min-h-screen w-full">
      <section className="mx-auto flex w-full max-w-6xl flex-col gap-8 px-4 py-10 md:px-8">
        <header className="flex flex-col gap-2">
          <h1 className="text-3xl font-semibold text-slate-900 md:text-4xl">
            Ventas
          </h1>
          <p className="text-base text-slate-500">
            Seguimiento del rendimiento de ventas por período.
          </p>
        </header>

        {/* Gráfica de línea */}
        <div className="rounded-xl border border-slate-200 bg-white p-6">
          <Tabs
            value={period}
            onValueChange={(v) => setPeriod(v as Period)}
            className="mb-6"
          >
            <TabsList>
              <TabsTrigger value="semana">Semana</TabsTrigger>
              <TabsTrigger value="mes">Mes</TabsTrigger>
              <TabsTrigger value="año">Año</TabsTrigger>
            </TabsList>
          </Tabs>

          <div className="h-80 w-full min-w-0">
            <ResponsiveContainer width="100%" height="100%">
              <LineChart
                data={data}
                margin={{ top: 5, right: 10, left: -10, bottom: 5 }}
              >
                <CartesianGrid
                  strokeDasharray="3 3"
                  stroke="#e2e8f0"
                  vertical={false}
                />
                <XAxis
                  dataKey="label"
                  axisLine={false}
                  tickLine={false}
                  tick={{ fill: "#64748b", fontSize: 12 }}
                  dy={10}
                />
                <YAxis
                  axisLine={false}
                  tickLine={false}
                  tick={{ fill: "#64748b", fontSize: 12 }}
                  tickCount={tickCount}
                />
                <Tooltip content={<CustomTooltip />} />
                <Line
                  type="monotone"
                  dataKey="ventas"
                  stroke={GREEN_COLOR}
                  strokeWidth={3}
                  dot={{
                    fill: GREEN_COLOR,
                    stroke: "#fff",
                    strokeWidth: 2,
                    r: 4,
                  }}
                  activeDot={{
                    fill: GREEN_COLOR,
                    stroke: "#fff",
                    strokeWidth: 2,
                    r: 6,
                  }}
                />
              </LineChart>
            </ResponsiveContainer>
          </div>
        </div>

        {/* Ventas por categoría */}
        <div className="rounded-xl border border-slate-200 bg-white p-6">
          <div className="mb-6">
            <h2 className="text-lg font-semibold text-slate-900">
              Ventas por categoría
            </h2>
            <p className="text-sm text-slate-500">
              Distribución del total de ventas por categoría.
            </p>
          </div>

          <div className="flex flex-col items-center gap-8 lg:flex-row lg:items-start">
            {/* Gráfica circular */}
            <div className="relative h-72 w-72 shrink-0">
              <svg
                className="h-full w-full"
                viewBox="0 0 100 100"
                xmlns="http://www.w3.org/2000/svg"
              >
                {(() => {
                  const radius = 40;
                  const cx = 50;
                  const cy = 50;
                  let currentAngle = -90;
                  const circumference = 2 * Math.PI * radius;

                  return categorySalesData.map((item, index) => {
                    const percentage = item.value / totalSales;
                    const segmentLength = percentage * circumference;
                    const dashOffset = -currentAngle * (circumference / 360);

                    currentAngle += percentage * 360;

                    return (
                      <circle
                        key={index}
                        cx={cx}
                        cy={cy}
                        r={radius}
                        fill="none"
                        stroke={GREEN_COLOR}
                        strokeWidth="12"
                        strokeDasharray={`${segmentLength} ${circumference}`}
                        strokeDashoffset={dashOffset}
                        opacity={0.2 + percentage * 0.8}
                        style={{ transition: "opacity 0.3s ease" }}
                      />
                    );
                  });
                })()}
              </svg>

              {/* Centro absoluto */}
              <div className="pointer-events-none absolute inset-0 flex flex-col items-center justify-center">
                <span className="text-sm font-medium text-slate-500">
                  Total de ventas
                </span>
                <span className="text-2xl font-bold text-slate-900">
                  {totalSales}
                </span>
              </div>
            </div>

            {/* Lista de categorías */}
            <div className="flex flex-1 flex-col gap-3">
              {categorySalesData.map((item) => {
                const percentage = (item.value / totalSales) * 100;
                return (
                  <div
                    key={item.name}
                    className="flex flex-col gap-1.5 rounded-lg border border-slate-100 bg-slate-50 px-4 py-3 transition hover:bg-slate-100"
                  >
                    <div className="flex items-center justify-between">
                      <span className="text-sm font-medium text-slate-700">
                        {item.name}
                      </span>
                      <div className="flex items-center gap-3">
                        <span className="text-sm font-semibold text-slate-900">
                          {item.value}
                        </span>
                        <span className="text-xs font-medium text-slate-500">
                          {percentage.toFixed(1)}%
                        </span>
                      </div>
                    </div>
                    {/* Barra de progreso */}
                    <div className="h-2 w-full overflow-hidden rounded-full bg-slate-200">
                      <div
                        className="h-full rounded-full transition-all"
                        style={{
                          width: `${percentage}%`,
                          backgroundColor: GREEN_COLOR,
                        }}
                      />
                    </div>
                  </div>
                );
              })}
            </div>
          </div>
        </div>
      </section>
    </main>
  );
}
