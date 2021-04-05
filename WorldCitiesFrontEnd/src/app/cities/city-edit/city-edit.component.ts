import { Component, OnInit } from '@angular/core';
import {AbstractControl, AsyncValidatorFn, FormControl, FormGroup, Validators} from '@angular/forms';
import {City} from '../city';
import {HttpClient, HttpParams} from '@angular/common/http';
import {ActivatedRoute, Router} from '@angular/router';
import {Country} from '../../countries/country';
import {Observable} from 'rxjs';
import {map} from 'rxjs/operators';
import {FormBaseComponent} from '../../shared/form-base/form-base.component';

@Component({
  selector: 'app-city-edit',
  templateUrl: './city-edit.component.html',
  styleUrls: ['./city-edit.component.css']
})
export class CityEditComponent extends FormBaseComponent implements OnInit {
  baseUrl = 'https://localhost:44348/api/';
  title: string;
  countries: Country[];
  form: FormGroup;
  city: City;
  id? : number;

  constructor(private http: HttpClient, private activateRoute: ActivatedRoute, private router: Router) {
    super();
  }

  ngOnInit(): void {
    this.form = new FormGroup({
      name: new FormControl('', Validators.required),
      latitude: new FormControl('', [Validators.required,  Validators.pattern(/^[-]?[0-9]+(\.[0-9]{1,4})?$/)]),
      longitude: new FormControl('', [Validators.required, Validators.pattern(/^[-]?[0-9]+(\.[0-9]{1,4})?$/)]),
      countryId: new FormControl('', Validators.required)
    }, null, this.isDuplicateCity())

    this.loadData();
  }
  loadCountries(){
    const url = this.baseUrl + 'countries';
    let params = new HttpParams()
      .set('pageSize', "9999")
      .set('pageIndex', '0')
      .set('sortColumn', 'name');
    this.http.get<any>(url, { params }).subscribe( result => {
      this.countries = result.data;
    }, error => console.error(error));
  }
  loadData(){
    this.loadCountries();

    this.id = +this.activateRoute.snapshot.paramMap.get('id');
    if(this.id){
      const url = this.baseUrl + 'cities/' + this.id;
      this.http.get<City>(url).subscribe( result => {
        this.city = result;
        this.title = 'Edit - ' + this.city.name;

        this.form.patchValue(this.city);
      }, error => console.log(error));
    }
    else{
      this.title = 'Create a new City';
    }

  }

  onSubmit() {
    let city = this.id ? this.city : <City>{};
    city.name = this.form.get('name').value;
    city.longitude = this.form.get('longitude').value;
    city.latitude = this.form.get('latitude').value;
    city.countryId = +this.form.get('countryId').value;

    if(this.id){
      const url = this.baseUrl + 'cities/' + this.city.id;
      this.http.put<City>(url, city).subscribe( result => {
        console.log('City ', + city.id + ' has been updated');

        this.router.navigate(['/cities']).then();
      }, error => console.error(error));
    }else {
      const url = this.baseUrl + 'cities';
      this.http.post<City>(url, city).subscribe( result => {
        console.log('city ' + result.id + ' has been created');
        this.router.navigate(['/cities']).then();
      }, error => console.error(error));
    }
  }

  isDuplicateCity() : AsyncValidatorFn {
    return (control: AbstractControl) : Observable< { [key: string]: any} | null> => {
      let city = <City>{};
      city.id = this.id ? this.id : 0 ;
      city.name = this.form.get('name').value;
      city.latitude = this.form.get('latitude').value;
      city.longitude = this.form.get('longitude').value;
      city.countryId = this.form.get('countryId').value;

      const url = this.baseUrl + 'cities/isDuplicateCity'
      return this.http.post<boolean>(url, city).pipe(map(result => {
        return (result  ? { isDuplicateCity : true} : null);
      }));
    }

  }
}
