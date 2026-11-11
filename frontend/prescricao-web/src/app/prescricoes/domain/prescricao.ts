export type TipoPrescricao = 'NaoEstruturada' | 'Estruturada';

export const TIPOS_PRESCRICAO: { valor: TipoPrescricao; rotulo: string }[] = [
  { valor: 'NaoEstruturada', rotulo: 'Não estruturada' },
  { valor: 'Estruturada', rotulo: 'Estruturada' },
];

export const LIMITE_TEXTO_LIVRE = 4000;
export const LIMITE_OBSERVACAO = 500;
export const MIN_ITENS = 1;
export const MAX_ITENS = 30;
export const MIN_DURACAO_DIAS = 1;
export const MAX_DURACAO_DIAS = 365;
export const TAMANHO_MAXIMO_PAGINA = 50;

export interface ItemPrescricao {
  id: string;
  medicamentoId: string | null;
  nomeMedicamento: string;
  medicamentoCadastrado: boolean;
  formaFarmaceutica: string;
  doseQuantidade: number;
  doseUnidade: string;
  frequencia: string;
  viaAdministracao: string;
  dataInicio: string;
  duracaoDias: number | null;
  usoContinuo: boolean;
  dataLimiteUso: string | null;
  observacao: string | null;
}

export interface Prescricao {
  id: string;
  pacienteId: string;
  nomePaciente: string;
  tipo: TipoPrescricao;
  nomeProfissional: string;
  registroProfissional: string;
  textoLivre: string | null;
  emitidaEmUtc: string;
  atualizadaEmUtc: string | null;
  itens: ItemPrescricao[];
}

/** Linha da listagem (OData /odata/Prescricoes). */
export interface PrescricaoResumo {
  id: string;
  pacienteId: string;
  nomePaciente: string;
  tipo: TipoPrescricao;
  nomeProfissional: string;
  registroProfissional: string;
  emitidaEmUtc: string;
  atualizadaEmUtc: string | null;
  quantidadeItens: number;
}

export interface Paciente {
  id: string;
  nomeCompleto: string;
  dataNascimento: string;
  sexo: string;
}

export interface Medicamento {
  id: string;
  nome: string;
  principioAtivo: string;
  formaFarmaceutica: string;
  concentracao: string;
}

export interface ItemPrescricaoEntrada {
  medicamentoId?: string;
  nomeMedicamentoLivre?: string;
  formaFarmaceutica: string;
  doseQuantidade: number;
  doseUnidade: string;
  frequencia: string;
  viaAdministracao: string;
  /** yyyy-MM-dd */
  dataInicio: string;
  duracaoDias?: number;
  usoContinuo: boolean;
  dataLimiteUso?: string;
  observacao?: string;
}

export interface CriarPrescricaoEntrada {
  pacienteId: string;
  tipo: TipoPrescricao;
  nomeProfissional: string;
  registroProfissional: string;
  textoLivre?: string;
  itens?: ItemPrescricaoEntrada[];
}

export type AtualizarPrescricaoEntrada = Omit<CriarPrescricaoEntrada, 'pacienteId'>;

export type Ordenacao = 'asc' | 'desc';

export interface FiltroPrescricoes {
  nomePaciente: string;
  tipo: TipoPrescricao | '';
  ordem: Ordenacao;
  /** Página começando em 1. */
  pagina: number;
  tamanhoPagina: number;
}

export interface Pagina<T> {
  total: number;
  itens: T[];
}
