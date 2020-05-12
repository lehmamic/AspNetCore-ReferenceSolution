import { CommonModule } from '@angular/common';
import { HttpClient, HttpClientModule } from '@angular/common/http';
import { NgModule } from '@angular/core';
import { IModuleTranslationOptions, ModuleTranslateLoader } from '@larscom/ngx-translate-module-loader';
import { TranslateLoader, TranslateModule, TranslateService } from '@ngx-translate/core';

import { DemoRoutingModule } from './demo-routing.module';
import { DemoComponent } from './demo/demo.component';

export function HttpLoaderFactory(http: HttpClient) {
  const baseTranslateUrl = './assets/i18n';

  const options: IModuleTranslationOptions = {
    modules: [
      { baseTranslateUrl, moduleName: 'shared' },
      { baseTranslateUrl, moduleName: 'demo' },
    ],
  };

  return new ModuleTranslateLoader(http, options);
}

@NgModule({
  declarations: [DemoComponent],
  imports: [
    // Angular Modules
    CommonModule,
    HttpClientModule,

    // 3rd Party Modules
    TranslateModule.forChild({
      defaultLanguage: 'de-CH',
      loader: {
        provide: TranslateLoader,
        useFactory: HttpLoaderFactory,
        deps: [HttpClient],
      },
      isolate: true,
    }),

    // App Modules
    DemoRoutingModule,
  ],
})
export class DemoModule {
  constructor(private translate: TranslateService) {
    // do this with ngrx
    this.translate.use('de-CH');
  }
}
