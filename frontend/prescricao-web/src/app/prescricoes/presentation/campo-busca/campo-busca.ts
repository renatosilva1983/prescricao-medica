import { Component, DestroyRef, OnInit, inject, input, output, signal } from '@angular/core';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { Observable, Subject, catchError, debounceTime, distinctUntilChanged, of, switchMap, tap } from 'rxjs';

let sequencial = 0;

/** Campo de texto com busca remota (debounce) e lista de sugestões. */
@Component({
  selector: 'app-campo-busca',
  template: `
    <div class="campo-busca">
      <input
        type="search"
        [id]="idCampo()"
        [value]="texto()"
        [placeholder]="placeholder()"
        [class.invalido]="invalido()"
        autocomplete="off"
        (input)="digitar($any($event.target).value)"
        (focus)="aberto.set(true)"
        (blur)="fecharDepois()"
      />
      @if (aberto() && (buscando() || resultados().length || (texto().length >= minimo() && !buscando()))) {
        <ul class="sugestoes" role="listbox" [id]="idLista">
          @if (buscando()) {
            <li class="vazio">Buscando…</li>
          } @else {
            @for (item of resultados(); track $index) {
              <li role="option" (mousedown)="escolher(item)">{{ rotulo()(item) }}</li>
            } @empty {
              <li class="vazio">Nenhum resultado.</li>
            }
          }
        </ul>
      }
    </div>
  `,
})
export class CampoBusca<T> implements OnInit {
  readonly buscar = input.required<(termo: string) => Observable<T[]>>();
  readonly rotulo = input.required<(item: T) => string>();
  readonly textoInicial = input('');
  readonly placeholder = input('Digite para buscar…');
  readonly minimo = input(2);
  readonly invalido = input(false);
  readonly idCampo = input(`campo-busca-${++sequencial}`);

  readonly selecionado = output<T>();
  /** Emitido quando o texto muda após uma seleção (a seleção deixa de valer). */
  readonly limpo = output<void>();

  protected readonly idLista = `sugestoes-${sequencial}`;
  protected readonly texto = signal('');
  protected readonly resultados = signal<T[]>([]);
  protected readonly buscando = signal(false);
  protected readonly aberto = signal(false);

  private readonly termos = new Subject<string>();
  private readonly destroyRef = inject(DestroyRef);

  ngOnInit(): void {
    this.texto.set(this.textoInicial());
    this.termos
      .pipe(
        debounceTime(300),
        distinctUntilChanged(),
        tap((t) => this.buscando.set(t.length >= this.minimo())),
        switchMap((t) =>
          t.length >= this.minimo() ? this.buscar()(t).pipe(catchError(() => of([] as T[]))) : of([] as T[]),
        ),
        takeUntilDestroyed(this.destroyRef),
      )
      .subscribe((lista) => {
        this.resultados.set(lista);
        this.buscando.set(false);
      });
  }

  protected digitar(valor: string): void {
    this.texto.set(valor);
    this.aberto.set(true);
    this.limpo.emit();
    this.termos.next(valor.trim());
  }

  protected escolher(item: T): void {
    this.texto.set(this.rotulo()(item));
    this.aberto.set(false);
    this.resultados.set([]);
    this.selecionado.emit(item);
  }

  protected fecharDepois(): void {
    setTimeout(() => this.aberto.set(false), 150);
  }
}
