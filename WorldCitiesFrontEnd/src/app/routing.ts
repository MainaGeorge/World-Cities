import {RouterModule, Routes} from '@angular/router';
import {HomeComponent} from './home/home.component';
import {NgModule} from '@angular/core';
import {CitiesComponent} from './cities/cities.component';
import {CountriesComponent} from './countries/countries.component';
import {CityEditComponent} from './cities/city-edit/city-edit.component';
import {CountryEditComponent} from './countries/country-edit/country-edit.component';


const routes: Routes = [
  {path: '', component: HomeComponent, pathMatch: 'full'},
  {path: 'cities', component: CitiesComponent},
  {path: 'countries', component: CountriesComponent},
  {path: 'city/:id', component: CityEditComponent},
  {path: 'city', component: CityEditComponent},
  {path: 'country/:id', component: CountryEditComponent},
  {path: 'country', component: CountryEditComponent}
];

@NgModule({
    imports: [
      RouterModule.forRoot(routes)
    ],
    exports: [
      RouterModule
    ]
  })
export class RoutingModule {}
