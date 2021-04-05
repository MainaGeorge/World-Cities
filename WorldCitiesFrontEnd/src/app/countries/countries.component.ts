import {Component, OnInit, ViewChild} from '@angular/core';
import {MatTableDataSource} from '@angular/material/table';
import {Country} from './country';
import {MatPaginator, PageEvent} from '@angular/material/paginator';
import {MatSort} from '@angular/material/sort';
import {HttpClient, HttpParams} from '@angular/common/http';
import {City} from '../cities/city';
import {debounceTime, distinctUntilChanged} from 'rxjs/operators';
import {Subject} from 'rxjs';

@Component({
  selector: 'app-countries',
  templateUrl: './countries.component.html',
  styleUrls: ['./countries.component.css']
})
export class CountriesComponent implements OnInit {
  baseUrl = 'https://localhost:44348/api/';
  public displayedColumns = ['id', 'name', 'iso2', 'iso3', 'totCities'];
  public countries: MatTableDataSource<Country>;
  defaultPageIndex = 0;
  defaultPageSize = 10;
  public defaultSortColumn = 'name';
  public defaultSortOrder = 'asc';

  defaultFilterColumn = 'name';
  filterQuery: string = null;
  filterTextChanged: Subject<string> = new Subject<string>();


  @ViewChild(MatPaginator) paginator: MatPaginator;
  @ViewChild(MatSort) sort: MatSort;

  constructor(private http: HttpClient) {
  }

  ngOnInit(): void {
    this.loadData();
  }

  getData(pageEvent: PageEvent) {
    const url = this.baseUrl + 'countries';
    let params = new HttpParams()
      .set('pageIndex', pageEvent.pageIndex.toString())
      .set('pageSize', pageEvent.pageSize.toString())
      .set('sortColumn', this.sort ? this.sort.active : this.defaultSortColumn)
      .set('sortOrder', this.sort ? this.sort.direction : this.defaultSortOrder);

    if (this.filterQuery) {
      params = params.set('filterColumn', this.defaultFilterColumn)
        .set('filterQuery', this.filterQuery);
    }
    this.http.get<any>(url, {params}).subscribe(result => {
      this.paginator.length = result.totalCount;
      this.paginator.pageIndex = result.pageIndex;
      this.paginator.pageSize = result.pageSize;
      this.countries = new MatTableDataSource<Country>(result.data);
    }, error => console.log(error));

  }

  loadData(query: string = null) {
    const pageEvent = new PageEvent();
    pageEvent.pageIndex = this.defaultPageIndex;
    pageEvent.pageSize = this.defaultPageSize;

    if (query) this.filterQuery = query;
    this.getData(pageEvent);
  }

  onFilterTextChanged(filterText: string) {
    if (this.filterTextChanged.observers.length === 0) {
      this.filterTextChanged.pipe(debounceTime(1000), distinctUntilChanged())
        .subscribe(query => this.loadData(query));
    }
    this.filterTextChanged.next(filterText);
  }
}
