import {Component, OnInit, ViewChild} from '@angular/core';
import {HttpClient, HttpParams} from '@angular/common/http';
import {City} from './city';
import {MatTableDataSource} from '@angular/material/table';
import {MatPaginator, PageEvent} from '@angular/material/paginator';
import {MatSort} from '@angular/material/sort';
import {Country} from '../countries/country';

@Component({
  selector: 'app-cities',
  templateUrl: './cities.component.html',
  styleUrls: ['./cities.component.css']
})
export class CitiesComponent implements OnInit {
  baseUrl = 'https://localhost:44348/api/';
  public cities: MatTableDataSource<City>;
  public displayedColumns: string[] = ['id', 'name', 'latitude', 'longitude'];
  defaultPageIndex = 0;
  defaultPageSize = 10;
  public defaultSortColumn = 'name';
  public defaultSortOrder = 'asc';

  defaultFilterColumn = 'name';
  filterQuery: string = null;

  @ViewChild(MatPaginator) paginator: MatPaginator;
  @ViewChild(MatSort) sort: MatSort;
  constructor(private http: HttpClient) { }

  ngOnInit(): void {
    this.loadData();
  }

  getData(pageEvent: PageEvent) {
    const url = this.baseUrl + 'cities';
    let params = new HttpParams()
      .set('pageIndex', pageEvent.pageIndex.toString())
      .set('pageSize', pageEvent.pageSize.toString())
      .set('sortColumn', this.sort ? this.sort.active : this.defaultSortColumn)
      .set('sortOrder', this.sort ? this.sort.direction : this.defaultSortOrder);

    if(this.filterQuery){
      params = params.set('filterColumn', this.defaultFilterColumn)
        .set('filterQuery', this.filterQuery);
    }
    this.http.get<any>(url, { params}).subscribe( result => {
      this.paginator.length = result.totalCount;
      this.paginator.pageIndex = result.pageIndex;
      this.paginator.pageSize = result.pageSize;
      this.cities = new MatTableDataSource<City>(result.data);
    }, error => console.log(error));

  }

  loadData(query: string = null) {
    const pageEvent = new PageEvent();
    pageEvent.pageIndex = this.defaultPageIndex;
    pageEvent.pageSize = this.defaultPageSize;

    if(query) this.filterQuery = query;
    this.getData(pageEvent);
  }
}
