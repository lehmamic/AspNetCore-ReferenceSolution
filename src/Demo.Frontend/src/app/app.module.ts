import { HttpClient, HttpClientModule, HTTP_INTERCEPTORS } from '@angular/common/http';
import { NgModule } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';
import { IModuleTranslationOptions, ModuleTranslateLoader } from '@larscom/ngx-translate-module-loader';
import { EffectsModule } from '@ngrx/effects';
import { RouterState, StoreRouterConnectingModule } from '@ngrx/router-store';
import { StoreModule } from '@ngrx/store';
import { StoreDevtoolsModule } from '@ngrx/store-devtools';
import { TranslateLoader, TranslateModule } from '@ngx-translate/core';

import { environment } from '../environments/environment';

import { AppRoutingModule } from './app-routing.module';
import { AppComponent } from './app.component';
import { AppEffects } from './app.effects';
import { metaReducers, reducers } from './app.reducers';
import { AngularCorrelationIdHttpInterceptor } from './utils/http/http/correlation-id.interceptor';
import { AngularDateHttpInterceptor } from './utils/http/http/date-parsing.interceptor';

export function HttpLoaderFactory(http: HttpClient) {
  const baseTranslateUrl = './assets/i18n';

  const options: IModuleTranslationOptions = {
    modules: [
      { baseTranslateUrl },
      { baseTranslateUrl, moduleName: 'shared' },
    ],
  };

  return new ModuleTranslateLoader(http, options);
}

@NgModule({
  declarations: [
    AppComponent,
  ],
  imports: [
    // Angular Modules
    BrowserModule,
    HttpClientModule,

    // 3rd Party Modules
    StoreModule.forRoot(reducers, {
      metaReducers,
    }),
    StoreDevtoolsModule.instrument({
      maxAge: 25, // Retains last 25 states
      logOnly: environment.production, // Restrict extension to log-only mode
    }),
    StoreRouterConnectingModule.forRoot({
      stateKey: 'router',
    }),
    EffectsModule.forRoot([
      AppEffects,
    ]),
    TranslateModule.forRoot({
      defaultLanguage: 'de-CH',
      loader: {
        provide: TranslateLoader,
        useFactory: HttpLoaderFactory,
        deps: [HttpClient],
      },
    }),

    // Application Modules
    AppRoutingModule,
  ],
  providers: [
    { provide: HTTP_INTERCEPTORS, useClass: AngularCorrelationIdHttpInterceptor, multi: true },
    { provide: HTTP_INTERCEPTORS, useClass: AngularDateHttpInterceptor, multi: true },
  ],
  bootstrap: [AppComponent],
})
export class AppModule { }
